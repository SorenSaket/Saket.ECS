using Saket.ECS.collections;

namespace Saket.ECS
{
    /// <summary>
    /// 
    /// </summary>
    public class Archetype
    {
        // Hashcode, ID
        // ArchetypeIds are equal across application at runtime, but differ each run
        public static Dictionary<int, int> ArchetypeIds = new Dictionary<int, int>();



        // idk
        public const int maxChunkSizeInBytes = 1024 * 16;

        public int id;

        public int Count;

        IComponentStorage[] storage;
        public Type[] ComponentTypes { get; private set; }
        private int componentHash = 0;

        public Stack<int> avaliableRows = new Stack<int>();

        public Archetype(Type[] components)
        {
            ComponentTypes = components;
            componentHash = GetComponentGroupHashCode(components);

            storage = new IComponentStorage[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                storage[i] = new ComponentStorage(1024);
            }
        }


        public void Add<T>(T value)
            where T : unmanaged
        {
            int index = Count;
            if(avaliableRows.Count > 0)
            {
                index = avaliableRows.Pop();
            }
            else
            {
                Count++;
            }

            Set<T>(index, value);
        }


        public T Get<T>(int index_element)
            where T : unmanaged
        {
            int index_component = IndexOfComponent<T>();

            if (index_component != -1)
            {
                return (T)storage[index_component].Get(index_element);
            }
            else
            {
                throw new Exception("Archetype does not contain component");
            }
        }

        public void Set<T>(int index_element, T value)
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

        public T Get<T>(int index_component, int index_element)
           where T : unmanaged
        {
            return storage[index_component].Get<T>(index_element);
        }


        private int IndexOfComponent<T>()
        {
            for (int i = 0; i < ComponentTypes.Length; i++)
            {
                if (ComponentTypes[i] == typeof(T))
                    return i;
            }

            return -1;
        }


        public int GetID()
        {
            return componentHash;
        }

        public static int GetComponentGroupHashCode(Type[] components)
        {
            // Sort to remove order variance
            Array.Sort(components,(x,y) => x.Name.CompareTo(y.Name));

            var hash = new HashCode();
            for (int i = 0; i < components.Length; i++)
            {
                hash.Add( components[i]);
            }
            return hash.ToHashCode();
        }
    }
}
