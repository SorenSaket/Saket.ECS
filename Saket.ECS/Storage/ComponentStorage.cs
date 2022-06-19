using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS.collections
{
    public enum ComponentStorageType
    {
        AOS,
        SOA
    }

    [AttributeUsage(AttributeTargets.Struct)]
    public class ComponentStorage : Attribute
    {
        public readonly ComponentStorageType Type;
        public ComponentStorage(ComponentStorageType type)
        {
            this.Type = type;
        }
    }


    public interface IComponentStorage
    {
        public Type ComponentType { get; }
        object Get(int index);
        void Set(int index, object value);
        void Recycle(int index);
        int New();
        void CopyData(int sourceIndex, int destinationIndex);
    }

    public class ComponentStorage : IComponentStorage
    {
        public Type ComponentType { get; private set; }

        public int Count { get; private set; } = 0;

        List<IntPtr> chunks;

        private int chunkSize;

        public ComponentStorage(Type component)
        {
            this.ComponentType = component;


            this.chunkSize = (Environment.SystemPageSize / Marshal.SizeOf(component));
            chunks = new List<IntPtr>();
        }

        public void Add<T>(T item)
        {
            if (chunks.Count >= chunkSize * chunks.Count)
            {
                chunks.Add(new T[chunkSize]);
            }

            Set(Count, item);

            Count++;
        }


        public void Set(int index, T item)
        {
            GetIndexes(index, out int index_chunk, out int index_element);

            chunks[index_chunk][index_element] = item;
        }


        public ref T Get(int index)
        {
            GetIndexes(index, out int index_chunk, out int index_element);

            return ref chunks[index_chunk][index_element];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetIndexes(int index, out int index_chunk, out int index_element)
        {
            index_chunk = index / chunkSize;
            index_element = index % chunkSize;
        }
    }
}
