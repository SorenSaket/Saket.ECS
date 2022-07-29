using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS.Storage
{
    public class ComponentStorageSettings
    {

    }
    /// <summary>
    /// Default storage type for storing components
    /// Uses an exponentional chunking strategy to resize
    /// </summary>
    public unsafe class ComponentStorage : IComponentStorage, IDisposable
    {
        public Type ComponentType { get; private set; }
        
        public int Count { get; private set; } = 0;
        public int Capacity { get; private set; } = 0;

        private readonly int itemSizeInBytes;
        private readonly int numberOfItemsInChunk;
        private const int chunkSizeInBytes = 16 * 1024;

        private bool disposedValue;

        List<IntPtr> chunks;

        #region Construction

        public ComponentStorage(Type component, ComponentStorageSettings? settings = default(ComponentStorageSettings))
        {
            this.ComponentType = component;

            this.itemSizeInBytes = Marshal.SizeOf(component);
            this.numberOfItemsInChunk = (chunkSizeInBytes / itemSizeInBytes);

            this.chunks = new List<IntPtr>();
        }



        #endregion

        #region Destruction
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {


                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                for (int i = 0; i < chunks.Count; i++)
                {
                    Marshal.FreeHGlobal(chunks[i]);
                }

                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                // TODO: set large fields to null


                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~ComponentStorage()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Generic IComponentStorage
        public void Set<T>(in int index, in T item) where T : unmanaged
        {
#if DEBUG
            if (!typeof(T).Equals(ComponentType))
                throw new ArgumentException("Object is not the same type as storage");
#endif
            EnsureSize(index);
            GetIndexes(index, out int index_chunk, out int index_element);
            
            fixed(T* ptr = &item)
            {
                // Get pointer to item
                byte* ptrItem = (byte*)ptr;
                byte* ptrDestination = (byte*)chunks[index_chunk].ToPointer();
                // Copy all bytes
                for (int y = 0; y < itemSizeInBytes; y++)
                {
                    ptrDestination[index_element * itemSizeInBytes + y] = ptrItem[y];
                }
            }
        }
        public T Get<T>(in int index) where T : unmanaged
        {
#if DEBUG
            if (!typeof(T).Equals(ComponentType))
                throw new ArgumentException("Object is not the same type as storage");
#endif
            GetIndexes(index, out int index_chunk, out int index_element);
#if DEBUG
            if (index_chunk >= chunks.Count || index_element >= numberOfItemsInChunk)
                throw new ArgumentOutOfRangeException("Index out of range");
#endif
            T* p = (T*)chunks[index_chunk].ToPointer();
            return p[index_element];
        }
        #endregion

        public void CopyTo(int index, IntPtr destination)
        {
            GetIndexes(index, out int index_chunk, out int index_element);
            Buffer.MemoryCopy( (void*)((byte*)chunks[index_chunk].ToPointer() + itemSizeInBytes * index_element), destination.ToPointer(), itemSizeInBytes, itemSizeInBytes);
        }

        #region object based IComponentStorage

        //[DllImport("msvcrt.dll", SetLastError = false)]
       // static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);

        public void Set(int index, IntPtr item)
        {
            EnsureSize(index);
            GetIndexes(index, out int index_chunk, out int index_element);
            Buffer.MemoryCopy(item.ToPointer(), ((byte*)chunks[index_chunk].ToPointer() + itemSizeInBytes * index_element), itemSizeInBytes, itemSizeInBytes);
            // memcpy(chunks[index_chunk] + itemSizeInBytes * index_element, item, itemSizeInBytes);
            //Marshal.Copy(item, 0, chunks[index_chunk]+itemSizeInBytes*index_element, itemSizeInBytes);
        }
        public IntPtr Get(int index)
        {
            GetIndexes(index, out int index_chunk, out int index_element);

#if DEBUG
            if (index_chunk >= chunks.Count || index_element >= numberOfItemsInChunk)
                throw new ArgumentOutOfRangeException("Index out of range");
#endif

            byte* ptr_chunk = (byte*)chunks[index_chunk].ToPointer();
            byte* ptr_element = &ptr_chunk[index_element * itemSizeInBytes];

            return new IntPtr((void*)ptr_element);
        }

        #endregion


        #region Internal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetIndexes(int index, out int index_chunk, out int index_element)
        {
            index_chunk = index / numberOfItemsInChunk;
            index_element = index % numberOfItemsInChunk;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureSize(int requiredCapacity)
        {
            while (requiredCapacity >= numberOfItemsInChunk * chunks.Count)
            {
                chunks.Add(Marshal.AllocHGlobal(chunkSizeInBytes));
            }
        }

        

        #endregion

    }
}
