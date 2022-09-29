using Saket.ECS.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;



namespace Saket.ECS
{
    /// <summary>
    /// 
    /// </summary>
    public class Archetype : IEquatable<Archetype?>
    {
        // Hashcode, ID
        // ArchetypeIds are equal across application at runtime, but differ each run
        //public static Dictionary<int, int> ArchetypeIds = new Dictionary<int, int>();

        public int ID => componentHash;

        /// <summary> Current Number of Entities </summary>
        public int Count { get; private set; }
        
        /// <summary>The maximum number of entities</summary>
        public int Capacity { get; private set; }

        /// <summary> Used to recycle entity IDs </summary>
        public Stack<int> avaliableRows = new Stack<int>();

        /// <summary> The components stored in archetype. Cannot be changed after construction</summary>
        public readonly HashSet<Type> ComponentTypes;
        /// <summary> Hashcode of component composition </summary>
        private readonly int componentHash = 0;

        /// <summary> Where the components are stored </summary>
        internal Dictionary<Type, IComponentStorage> storage;

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
            if(avaliableRows.Count > 0)
            {
                Count++;
                return avaliableRows.Pop();
            }
            Capacity++;
            Count++;
            foreach (var store in storage)
            {
                store.Value.EnsureSize(Capacity);
            }

            return Count-1;
        }
        internal void RemoveEntity(int index_element)
        {
            if(index_element >= Count)
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

        internal T Get<T>(int index_element)
            where T : unmanaged
        {
            return storage[typeof(T)].Get<T>(index_element);
        }
        internal void Set<T>(int index_element, T value)
            where T : unmanaged
        {
            storage[typeof(T)].Set<T>(index_element, value);
        }


        internal unsafe void* Get(Type type, int index_element)
        {
            return storage[type].Get(index_element);
        }
        internal unsafe void Set(Type type, int index_element, void* value)
        {
           storage[type].Set(index_element, value);
        }
#endregion

#region Other
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
#endregion
    }
}
