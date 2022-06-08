using engine.ecs.collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.CompilerServices;

namespace engine.ecs
{
    /// <summary>
    /// 
    /// </summary>
    public class Archetype
    {
        // Hashcode, ID
        // ArchetypeIds are equal across application at runtime, but differ each run
        // T
        public static Dictionary<int, int> ArchetypeIds = new Dictionary<int, int>();

        // idk
        public const int maxChunkSizeInBytes = 1024 * 16;

        public int id;

        public int Count;


        ChunkedMultiArray[] storage;
        Type[] componentTypes;
        private int componentHash = 0;

        public Archetype(Type[] components)
        {
            componentTypes = components;
            componentHash = GetComponentGroupHashCode(components);

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
            for (int i = 0; i < componentTypes.Length; i++)
            {
                if (componentTypes[i] == typeof(T))
                    return i;
            }

            return -1;
        }


        public int GetID()
        {
            return componentHash;
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
