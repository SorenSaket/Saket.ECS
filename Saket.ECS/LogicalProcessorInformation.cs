using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    // https://docs.microsoft.com/en-us/windows/win32/api/sysinfoapi/nf-sysinfoapi-getlogicalprocessorinformation
    // https://stackoverflow.com/questions/6972437/pinvoke-for-getlogicalprocessorinformation-function
    public static class LogicalProcessorInformation
    {
        public static SYSTEM_LOGICAL_PROCESSOR_INFORMATION[]? Information;
        static LogicalProcessorInformation()
        {
            Information = GetLogicalProcessorInformation();
        }
        
        /// <summary>
        /// Represents the type of processor cache identified in the corresponding CACHE_DESCRIPTOR structure.
        /// </summary>
        public enum PROCESSOR_CACHE_TYPE
        {   /// <summary>
            /// The cache is unified.
            /// </summary>
            CacheUnified,
            /// <summary>
            /// The cache is for processor instructions.
            /// </summary>
            CacheInstruction,
            /// <summary>
            /// The cache is for data.
            /// </summary>
            CacheData,
            /// <summary>
            /// The cache is for traces.
            /// </summary>
            CacheTrace
        }

        /// <summary>
        /// Describes the cache attributes.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CACHE_DESCRIPTOR
        {
            /// <summary>
            /// The cache level. This member can currently be one of the following values; 1, 2, 3
            /// </summary>
            public byte Level;
            /// <summary>
            /// The cache associativity. If this member is CACHE_FULLY_ASSOCIATIVE (0xFF), the cache is fully associative.
            /// </summary>
            public byte Associativity;
            /// <summary>
            /// The cache line size, in bytes.
            /// </summary>
            public ushort LineSize;
            /// <summary>
            /// The cache size, in bytes.
            /// </summary>
            public uint Size;
            /// <summary>
            /// The cache type. This member is a PROCESSOR_CACHE_TYPE value.
            /// </summary>
            public PROCESSOR_CACHE_TYPE Type;
        }

        /// <summary>
        /// This structure contains valid data only if the Relationship member is RelationProcessorCore.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSORCORE
        {
            /// <summary>
            /// If the value of this member is 1, the logical processors identified by the value of the ProcessorMask member share functional units, as in Hyperthreading or SMT. Otherwise, the identified logical processors do not share functional units.
            /// </summary>
            public byte Flags;
        };

        /// <summary>
        /// This structure contains valid data only if the Relationship member is RelationNumaNode.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NUMANODE
        {
            /// <summary>
            /// Identifies the NUMA node. The valid values of this parameter are 0 to the highest NUMA node number inclusive. A non-NUMA multiprocessor system will report that all processors belong to one NUMA node.
            /// </summary>
            public uint NodeNumber;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION_UNION
        {
            [FieldOffset(0)]
            public PROCESSORCORE ProcessorCore;
            [FieldOffset(0)]
            public NUMANODE NumaNode;
            [FieldOffset(0)]
            public CACHE_DESCRIPTOR Cache;
            [FieldOffset(0)]
            private UInt64 Reserved1;
            [FieldOffset(8)]
            private UInt64 Reserved2;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum LOGICAL_PROCESSOR_RELATIONSHIP
        {
            /// <summary>
            /// The specified logical processors share a single processor core.
            /// </summary>
            RelationProcessorCore,
            /// <summary>
            /// The specified logical processors are part of the same NUMA node.
            /// </summary>
            RelationNumaNode,
            /// <summary>
            /// The specified logical processors share a cache.
            /// </summary>
            RelationCache,
            /// <summary>
            /// The specified logical processors share a physical package (a single package socketed or soldered onto a motherboard may contain multiple processor cores or threads, each of which is treated as a separate processor by the operating system).
            /// </summary>
            RelationProcessorPackage,
            /// <summary>
            /// The specified logical processors share a single processor group.
            /// </summary>
            RelationGroup,
            /// <summary>
            /// On input, retrieves information about all possible relationship types. This value is not used on output.
            /// </summary>
            RelationAll = 0xffff
        }

        /// <summary>
        /// Describes the relationship between the specified processor set. This structure is used with the GetLogicalProcessorInformation function.
        /// </summary>
        public struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION
        {
            /// <summary>
            /// The processor mask identifying the processors described by this structure. A processor mask is a bit vector in which each set bit represents an active processor in the relationship. At least one bit will be set.
            /// On a system with more than 64 processors, the processor mask identifies processors in a single processor group.
            /// </summary>
            public UIntPtr ProcessorMask;
            /// <summary>
            /// The relationship between the processors identified by the value of the ProcessorMask member.
            /// </summary>
            public LOGICAL_PROCESSOR_RELATIONSHIP Relationship;
            /// <summary>
            /// 
            /// </summary>
            public SYSTEM_LOGICAL_PROCESSOR_INFORMATION_UNION ProcessorInformation;
        }




        /// <summary>
        /// Retrieves information about logical processors and related hardware.
        /// </summary>
        /// <param name="Buffer"> A pointer to a buffer that receives an array of SYSTEM_LOGICAL_PROCESSOR_INFORMATION structures. If the function fails, the contents of this buffer are undefined. </param>
        /// <param name="ReturnLength">On input, specifies the length of the buffer pointed to by Buffer, in bytes. If the buffer is large enough to contain all of the data, this function succeeds and ReturnLength is set to the number of bytes returned. If the buffer is not large enough to contain all of the data, the function fails, GetLastError returns ERROR_INSUFFICIENT_BUFFER, and ReturnLength is set to the buffer length required to contain all of the data. If the function fails with an error other than ERROR_INSUFFICIENT_BUFFER, the value of ReturnLength is undefined.</param>
        /// <returns>If the function succeeds, the return value is TRUE and at least one SYSTEM_LOGICAL_PROCESSOR_INFORMATION structure is written to the output buffer. If the function fails, the return value is FALSE.To get extended error information, call GetLastError.</returns>
        [DllImport(@"kernel32.dll", SetLastError = true)]
        private static extern bool GetLogicalProcessorInformation(
            
            IntPtr Buffer,

            ref uint ReturnLength
        );

        private const int ERROR_INSUFFICIENT_BUFFER = 122;
        private static SYSTEM_LOGICAL_PROCESSOR_INFORMATION[]? GetLogicalProcessorInformation()
        {
            uint ReturnLength = 0;

            // Made to fail
            GetLogicalProcessorInformation(IntPtr.Zero, ref ReturnLength);

            // On fail allocate nessary buffer
            if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
            {
                IntPtr Ptr = Marshal.AllocHGlobal((int)ReturnLength);
                try
                {
                    if (GetLogicalProcessorInformation(Ptr, ref ReturnLength))
                    {
                        int size = Marshal.SizeOf(typeof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION));
                        int len = (int)ReturnLength / size;
                        SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] Buffer = new SYSTEM_LOGICAL_PROCESSOR_INFORMATION[len];
                        IntPtr Item = Ptr;
                        for (int i = 0; i < len; i++)
                        {
                            Buffer[i] = (SYSTEM_LOGICAL_PROCESSOR_INFORMATION)Marshal.PtrToStructure(Item, typeof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION));
                            Item += size;
                        }
                        return Buffer;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(Ptr);
                }
            }
            return null;
        }
    }
}
