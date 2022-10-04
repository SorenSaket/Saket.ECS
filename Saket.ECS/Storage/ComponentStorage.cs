using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS.Storage
{
    /// <summary>
    /// Default storage type for storing components
    /// </summary>
    public unsafe class ComponentStorage : IComponentStorage, IDisposable
    {
        public Type ComponentType { get; }
        public int Capacity { get; private set; } = 0;

        public int ItemSizeInBytes { get; }

        internal bool disposedValue;

        byte* data;

        #region Construction

        public ComponentStorage(Type component)
        {
#if DEBUG
            if (!Utilities.IsValidComponent(component))
                throw new Exception("Invalid Component");
#endif
            this.ComponentType = component;
            this.ItemSizeInBytes = Marshal.SizeOf(component);
            
            // Initial Allocation
            data = (byte*)Marshal.AllocHGlobal(8 * ItemSizeInBytes).ToPointer();
            Capacity = 8;
        }

        #endregion

        #region Destruction
        // https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Marshal.FreeHGlobal(new IntPtr(data));
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
        public void Set<T>(int index, in T item) where T : unmanaged
        {
#if DEBUG
            if (!typeof(T).Equals(ComponentType))
                throw new ArgumentException("Object is not the same type as storage");
            if (index >= Capacity || index < 0)
                throw new ArgumentOutOfRangeException("Index out of range");
#endif
            fixed (T* ptr = &item)
            {
                // Get pointer to item
                byte* ptrItem = (byte*)ptr;
                // Copy all bytes
                for (int y = 0; y < ItemSizeInBytes; y++)
                {
                    data[index * ItemSizeInBytes + y] = ptrItem[y];
                }
            }
        }
        // MIght use these in future
        // Marshal.PtrToStructure()
        // Marshal.WriteByte
        // Marshal.StructureToPtr
        // Marshal.ReAllocHGlobal
        // Marshal.PrelinkAll is interesting to avoid jit penalty runtime

        public T Get<T>(int index) where T : unmanaged
        {
#if DEBUG
            if (!typeof(T).Equals(ComponentType))
                throw new ArgumentException("Object is not the same type as storage");
            if (index >= Capacity || index < 0)
                throw new ArgumentOutOfRangeException("Index out of range");
#endif
            return ((T*)data)[index];
        }
        #endregion

        #region Pointer based IComponentStorage
        
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range</exception>
        public unsafe void Set(int index, void* item)
        {
#if DEBUG
            if (index >= Capacity || index < 0)
                throw new ArgumentOutOfRangeException("Index out of range");
#endif
            //
            Buffer.MemoryCopy(
                item, 
                &data[ItemSizeInBytes * index], 
                ItemSizeInBytes, 
                ItemSizeInBytes);
            // Alternative apis
            //[DllImport("msvcrt.dll", SetLastError = false)]
            // static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);
            // memcpy(chunks[index_chunk] + itemSizeInBytes * index_element, item, itemSizeInBytes);
            //Marshal.Copy(item, 0, chunks[index_chunk]+itemSizeInBytes*index_element, itemSizeInBytes);
        }

        /// <exception cref="ArgumentOutOfRangeException">The index is out of range</exception>
        public unsafe void* Get(int index)
        {
#if DEBUG
        if (index >= Capacity || index < 0)
            throw new ArgumentOutOfRangeException("Index out of range");
#endif
            return &data[index * ItemSizeInBytes];
        }

        #endregion


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureCapacity(int requiredCapacity)
        {
            int newCapacity = Capacity;
            // Double the capacity until theres enough
            while (requiredCapacity >= newCapacity)
            {
                newCapacity *= 2;
            }

            // https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.reallochglobal?view=net-6.0
            // "the original memory block has been freed"
            data = (byte*)Marshal.ReAllocHGlobal(new IntPtr(data), new IntPtr(newCapacity * ItemSizeInBytes)).ToPointer();
            Capacity = newCapacity;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Zero(int index)
        {
#if DEBUG
            if (index >= Capacity || index < 0)
                throw new ArgumentOutOfRangeException("Index out of range");
#endif
            for (int i = 0; i < ItemSizeInBytes; i++)
            {
                (data[ItemSizeInBytes * index]) = 0;
            }
        }
    }
}
