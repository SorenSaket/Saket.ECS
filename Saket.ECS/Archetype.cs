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
        public Type[] ComponentTypes { get; }

        /// <summary> Hashcode of component composition </summary>
        private readonly int componentHash = 0;

        /// <summary> Where the components are stored </summary>
        internal IComponentStorage[] storage;

        /// <summary>
        /// Create new Archetype store with desired Components
        /// </summary>
        /// <param name="components"></param>
        public Archetype(Type[] components)
        {
            ComponentTypes = components;
            componentHash = GetComponentGroupHashCode(components);

            storage = new IComponentStorage[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                // TODO: Check for storage type and use accordingly
                storage[i] = new ComponentStorage(components[i]);
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

        internal T Get<T>(int index_element)
            where T : unmanaged
        {
            int index_component = IndexOfComponent<T>();

            if (index_component != -1)
            {
                return storage[index_component].Get<T>(index_element);
            }
            else
            {
                throw new Exception("Archetype does not contain component");
            }
        }
        internal T Get<T>(int index_component, int index_element) where T : unmanaged
        {
            return storage[index_component].Get<T>(index_element);
        }
        internal IntPtr Get(int index_component, int index_element)
        {
            return storage[index_component].Get(index_element);
        }

        internal void Set<T>(int index_element, T value)
            where T : unmanaged
        {
            int index_component = IndexOfComponent<T>();

            if (index_component != -1)
            {
                storage[index_component].Set<T>(index_element, value);
            }
            else
            {
                throw new Exception("Archetype does not contain component");
            }
        }
        internal void Set(int index_component, int index_element, IntPtr value)
        {
           storage[index_component].Set(index_element, value);
        }
        #endregion


        #region Other
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int IndexOfComponent<T>()
        {
            for (int i = 0; i < ComponentTypes.Length; i++)
            {
                if (ComponentTypes[i] == typeof(T))
                    return i;
            }

            return -1;
        }


        public static int GetComponentGroupHashCode(Type[] components)
        {
            // Sort to remove order variance
            // TODO: is this needed?
            var h = new Type[components.Length];
            Array.Copy(components, h, components.Length);
            Array.Sort(h, (x,y) => x.Name.CompareTo(y.Name));
            // Remember that array is a refernce type so this will fuck up anything else in the callstack

            var hash = new HashCode();
            for (int i = 0; i < h.Length; i++)
            {
                hash.Add(h[i]);
            }
            return hash.ToHashCode();
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
