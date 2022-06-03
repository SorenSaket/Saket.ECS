using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace engine.ecs.collections
{
    public class ChunkedMultiArray
    {
        public int Count { get; private set; } = 0;

        List<MultiArray> data;

        private int chunkSize;

        public ChunkedMultiArray(int chunkSize)
        {
            this.chunkSize = chunkSize;
            data = new List<MultiArray>();
        }

        public void Add<T>(T item)
            where T : unmanaged
        {
            if(data.Count >= chunkSize*data.Count)
            {
                data.Add(new MultiArray(chunkSize, typeof(T)));
            }

            int chunkIndex = Count / chunkSize;
            int index = Count % chunkSize;

            data[chunkIndex].Set<T>(index, item);

            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetIndexes(int index, out int index_chunk, out int index_element)
        {
            index_chunk = index / chunkSize;
            index_element = index % chunkSize;
        }

        public void Set<T>(int index, T item)
             where T : unmanaged
        {
            
        }
        public T Get<T>(int index)
             where T : unmanaged
        {
            GetIndexes(index, out int index_chunk, out int index_element);

            return data[index_chunk].Get<T>(index_element);

        }

    }
}
