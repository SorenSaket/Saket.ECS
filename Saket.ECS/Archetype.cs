using engine.ecs.collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace engine.ecs
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


        ChunkedMultiArray[] storage;
        Type[] componentTypes;

        public Archetype(Type[] components)
        {
            componentTypes = components;


            storage = new ChunkedMultiArray[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                storage[i] = new ChunkedMultiArray(1024);
            }
        }

        public T Get<T>(int index_element)
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

        private int IndexOfComponent<T>()
        {
            for (int i = 0; i < componentTypes.Length; i++)
            {
                if (componentTypes[i] == typeof(T))
                    return i;
            }

            return -1;
        }

        // Does this work??
        public static int GetComponentGroupHashCode(Type[] components)
        {
            // Wrap around
            unchecked
            {
                int hash = 0;
                for (int i = 0; i < components.Length; i++)
                {
                    hash += components[i].GetHashCode();
                }
                return hash;
            }
        }

    }
}
