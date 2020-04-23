using System;
using System.Runtime.InteropServices;
using static NTMiner.Windows.Ram;

namespace NTMiner.Windows {
    internal static partial class SafeNativeMethods {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(DllName.Kernel32Dll, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GlobalMemoryStatusEx([In, Out]MemoryStatusEx lpBuffer);

        [DllImport(DllName.Kernel32Dll)]
        internal static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SystemInfo lpSystemInfo);

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        internal struct SystemInfo {
            /// <summary>
            /// Used to access the ProcessorInfoUnion struct
            /// </summary>
            internal readonly ProcessorInfoUnion uProcessorInfo;

            /// <summary>
            /// The page size and the granularity of page protection and commitment.
            /// </summary>
            internal readonly uint dwPageSize;

            /// <summary>
            /// A pointer to the lowest memory address accessible 
            /// to applications and dynamic-link libraries (DLLs).
            /// </summary>
            internal readonly IntPtr lpMinimumApplicationAddress;

            /// <summary>
            /// A pointer to the highest memory address accessible to applications and DLLs.
            /// </summary>
            internal readonly IntPtr lpMaximumApplicationAddress;

            /// <summary>
            /// A mask representing the set of processors configured into the system.
            /// Bit 0 is processor 0; bit 31 is processor 31.
            /// </summary>
            internal readonly IntPtr dwActiveProcessorMask;

            /// <summary>
            /// The number of logical processors in the current group.
            /// To retrieve this value, use the GetLogicalProcessorInformation function.
            /// </summary>
            internal readonly uint dwNumberOfProcessors;

            /// <summary>
            /// An obsolete member that is retained for compatibility. 
            /// 
            /// Use the wProcessorArchitecture, wProcessorLevel, and wProcessorRevision 
            /// members to determine the type of processor.
            /// </summary>
            [Obsolete("Use wProcessorArchitecture, wProcessorLevel and wProcessorRevision instead")]
            internal readonly uint dwProcessorType;

            /// <summary>
            /// The granularity for the starting address at which virtual memory can be allocated.
            /// </summary>
            internal readonly uint dwAllocationGranularity;

            /// <summary>
            /// The architecture-dependent processor level. 
            /// 
            /// It should be used only for display purposes.
            /// 
            /// If wProcessorArchitecture is PROCESSOR_ARCHITECTURE_INTEL, 
            /// wProcessorLevel is defined by the CPU vendor.
            /// 
            /// If wProcessorArchitecture is PROCESSOR_ARCHITECTURE_IA64, 
            /// wProcessorLevel is set to 1.
            /// </summary>
            internal readonly ushort dwProcessorLevel;

            /// <summary>
            /// The architecture-dependent processor revision. 
            /// </summary>
            internal readonly ushort dwProcessorRevision;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct ProcessorInfoUnion {
            /// <summary>
            /// An obsolete member that is retained for compatibility. 
            /// Applications should use the wProcessorArchitecture branch of the union.
            /// </summary>
            [Obsolete("Use the wProcessorArchitecture field instead")]
            [FieldOffset(0)]
            internal readonly uint dwOemId;

            /// <summary>
            /// The processor architecture of the installed operating system. 
            /// </summary>
            [FieldOffset(0)]
            internal readonly ushort wProcessorArchitecture;

            /// <summary>
            /// This member is reserved for future use.
            /// </summary>
            [FieldOffset(2)]
            internal readonly ushort wReserved;
        }


        #endregion
    }
}
