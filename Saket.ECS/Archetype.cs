using Saket.ECS.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;



namespace Saket.ECS
{
    /// <summary>
    /// 
    /// </summary>
    public class Archetype : IEquatable<Archetype?> , IEnumerable<int>
    {
        // Hashcode, ID
        // ArchetypeIds are equal across application at runtime, but differ each run
        //public static Dictionary<int, int> ArchetypeIds = new Dictionary<int, int>();

        public int ID => componentHash;

        /// <summary> Current Number of Entities </summary>
        public int Count { get; private set; }
        
        /// <summary> The minimum allocated number of entities. The acutual allocation is up to the storage</summary>
        public int Capacity { get; private set; }

        /// <summary> Version inceases when a entity is destroyed or created</summary>
        public uint Version { get; private set; }

        /// <summary> Used to recycle rows/entities </summary>
        public Stack<int> avaliableRows = new Stack<int>();

        /// <summary> The components stored in archetype. Cannot be changed after construction</summary>
        public HashSet<Type> ComponentTypes { get; protected set; }
        /// <summary> Hashcode of component composition </summary>
        private int componentHash = 0;

        /// <summary> Where the components are stored </summary>
        public Dictionary<Type, IComponentStorage> storage;

        /// <summary>
        /// Create new Archetype store with desired Components
        /// </summary>
        /// <param name="components"></param>
        public Archetype(HashSet<Type> components)
        {
            ComponentTypes = components;
            // Cache hashcode
            componentHash = GetComponentGroupHashCode(components);
            
            // Initialize storage
            storage = new Dictionary<Type, IComponentStorage>(components.Count);
            foreach (var component in components)
            {
                storage[component] = new ComponentStorage(component);
            }
        }

        internal int AddEntity()
        {
            InceaseVersion();
            /*TODO
             // Clear reused storage 
                int row = avaliableRows.Pop();
                foreach (var store in storage)
                {
                    store.Value.Zero(row);
                }
             */
            if (avaliableRows.Count > 0)
            {
                Count++;
                return avaliableRows.Pop();
            }
            Count++;
            Capacity++;
            foreach (var store in storage)
            {
                store.Value.EnsureCapacity(Capacity);
            }

            return Count-1;
        }
        internal void RemoveEntity(int index_element)
        {
            if(index_element >= Capacity)
            {
                throw new Exception("entity does not exist");
            }
            else if(avaliableRows.Contains(index_element))
            {
                throw new Exception("entity is already removed");
            }
            else
            {
                avaliableRows.Push(index_element);
                Count--;
                InceaseVersion();
            }
        }


        #region Component
        /// <summary>
        /// Whether or not the Archetype has component T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool Has<T>()
        {
            return ComponentTypes.Contains(typeof(T));
        }
        public bool Has(Type type)
        {
            return ComponentTypes.Contains(type);
        }

        public T Get<T>(int index_element)
            where T : unmanaged
        {
            return storage[typeof(T)].Get<T>(index_element);
        }
        public void Set<T>(int index_element, T value)
            where T : unmanaged
        {
            storage[typeof(T)].Set<T>(index_element, value);
        }


        public unsafe void* Get(Type type, int index_element)
        {
            return storage[type].Get(index_element);
        }
        public unsafe void Set(Type type, int index_element, void* value)
        {
           storage[type].Set(index_element, value);
        }
        #endregion

        #region Other

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InceaseVersion()
        {
            unchecked
            {
                Version++;
            }
        }

        public static int GetComponentGroupHashCode(HashSet<Type> components)
        {
            int hashCode = 31;
            // 
            foreach (var item in components)
            {
                hashCode ^= item.GetHashCode();
            }

            return hashCode;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Archetype);
        }
        public bool Equals(Archetype? other)
        {
            return other != null && this.ID == other.ID;
        }
        public override int GetHashCode()
        {
            return GetComponentGroupHashCode(ComponentTypes);
        }

        public IEnumerator<int> GetEnumerator()
        {
            return new ArchetypeEnumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Clears all entity related data
        /// Does not deallocate memory
        /// </summary>
        public void Clear()
        {
            Count = 0;
            Capacity = 0;
            avaliableRows.Clear();
        }

        /// <summary>
        /// Clone all storage to other archetype. Does not clone storages that does not exsist on other
        /// Ensure that other.ComponentTypes == ComponentTypes
        /// </summary>
        /// <param name="other"></param>
        public void Overwrite(Archetype other)
        {
            other.Count = Count;
            other.Capacity = Capacity;
            other.avaliableRows = new Stack<int>(avaliableRows);

            // Clone all storage
            foreach (var store in storage)
            {
                // Ensure the other storage exists on archetype
                if(other.storage.ContainsKey(store.Key))
                    store.Value.CloneTo(other.storage[store.Key]);
            }
            
        }

        #endregion
    }
}
