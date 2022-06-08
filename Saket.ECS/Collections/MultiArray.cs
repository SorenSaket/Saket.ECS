using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace engine.ecs.collections
{
    /// <summary>
    /// Array with automatic SOA layout
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public  class MultiArray
    {
        // 
        public int Length { get; private set; }


        // Umanaged Memory pointer
        IntPtr data;

        /// <summary> Holds the size in bytes for all fields </summary>
        System.Reflection.FieldInfo[] fields;
        /// <summary> Holds the size in bytes for all fields </summary>
        int[] sizes;
        /// <summary> Holds the offset in bytes for all fields in current data structure </summary>
        int[] offsets;
        /// <summary> Holds the offset in bytes for managed datastructure </summary>
        int[] localOffsets;

        // Constructor
        public MultiArray(int count, Type type)
        {
            Length = count;
            // Total size of a single element in bytes
            // The element size is not equals to Marshal.SizeOf(typeof(T))
            // Since each field is stored sequentially the is no padding
            // Therefore the element is computed
            int totalElementSize = 0;

            // Get the field of the type
            fields = type.GetFields();
           
            // Intialize arrays
            sizes = new int[fields.Length];
            offsets = new int[fields.Length];
            localOffsets = new int[fields.Length];

            // 
            for (int i = 0; i < fields.Length; i++)
            {
                // Offset in struct
                localOffsets[i] = Marshal.OffsetOf(type, fields[i].Name).ToInt32();
            }

            for (int i = 0; i < fields.Length; i++)
            {
                // Datastructure offset
                offsets[i] = totalElementSize * count;
                // The size in bytes for each field Type is either it's Marshal.SizeOf()
                // Or is determined by delta in explicit layout
                if (i != fields.Length - 1)
                    sizes[i] = Math.Min(Marshal.SizeOf(fields[i].FieldType), localOffsets[i + 1] - localOffsets[i]);
                else
                    sizes[i] = Marshal.SizeOf(fields[i].FieldType);
                // 
                totalElementSize += sizes[i];
            }

            // Allocate Memory
            data = Marshal.AllocHGlobal(totalElementSize*count);
        }
        // Destructor
        ~MultiArray()
        {
            // Free memory on destruction
            Marshal.FreeHGlobal(data);
        }
       

        public unsafe void Set<T>(int index, T item)
            where T : unmanaged
        {
            // Get pointer to item
            byte* ptrItem = (byte*)&item;
            // For each field
            for (int i = 0; i < fields.Length; i++)
            {
                // get pointer for field
                byte* ptrField = (byte*)GetFieldPointer(i);
                // Copy all bytes
                for (int y = 0; y < sizes[i]; y++)
                {
                    ptrField[index * sizes[i] + y] = ptrItem[localOffsets[i] + y];
                }
            }
        }
        public unsafe T Get<T>(int index)
            where T : unmanaged
        {
            T r = default(T);

            byte* a = (byte*)&r;

            for (int i = 0; i < fields.Length; i++)
            {
                byte* q = (byte*)GetFieldPointer(i);

                for (int y = 0; y < sizes[i]; y++)
                {
                    a[localOffsets[i] + y] = q[index * sizes[i] + y];
                }
            }
            return r;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void* GetFieldPointer(int field)
        {
            return (byte*)data.ToPointer() + offsets[field];
        }


        public static int GetStoredSizeinBytes(Type type)
        {
            int size = 0;
            // Get the field of the type
            var fields = type.GetFields();

            // Intialize arrays

            var offsets = new int[fields.Length];

            // 
            for (int i = 0; i < fields.Length; i++)
            {
                // Offset in struct
                offsets[i] = Marshal.OffsetOf(type, fields[i].Name).ToInt32();
            }

            for (int i = 0; i < fields.Length; i++)
            {
                // The size in bytes for each field Type is either it's Marshal.SizeOf()
                // Or is determined by delta in explicit layout
                if (i != fields.Length - 1)
                    size += Math.Min(Marshal.SizeOf(fields[i].FieldType), offsets[i + 1] - offsets[i]);
                else
                    size += Marshal.SizeOf(fields[i].FieldType);
            }
            return size;
        }
    }
}