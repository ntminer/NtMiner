using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NTMiner.Windows {
    /// <summary>
    /// Class for retrieving information related to the processor
    /// </summary>
    public sealed class Processor {
        public static readonly Processor Current = new Processor();

        // This stores the total number of logical cores in the processor
        private readonly int numberOfProcessors = Environment.ProcessorCount;

        #region Properties

        /// <summary>
        /// Gets the name of the processor.
        /// </summary>
        public string Name {
            get {
                return RetrieveProcessorInfo("ProcessorNameString");
            }
        }

        /// <summary>
        /// Retrieves the number of logical cores on the system
        /// </summary>
        public int NumberOfLogicalCores {
            get {
                return numberOfProcessors;
            }
        }

        /// <summary>
        /// Gets the clock speed in MHz
        /// </summary>
        public string ClockSpeed {
            get {
                return RetrieveProcessorInfo("~MHz") + " MHz";
            }
        }

        /// <summary>
        /// Gets the name of the processor.
        /// </summary>
        public string Identifier {
            get {
                return RetrieveProcessorInfo("Identifier");
            }
        }

        /// <summary>
        /// Gets the vendor identifier of the processor.
        /// </summary>
        public string VendorIdentifier {
            get {
                return RetrieveProcessorInfo("VendorIdentifier");
            }
        }

        /// <summary>
        /// Gets the architecture-dependent processor level. 
        /// </summary>
        public string ProcessorLevel {
            get {
                ProcessorInfo.SystemInfo pInfo = new ProcessorInfo.SystemInfo();
                ProcessorInfo.GetSystemInfo(ref pInfo);
                return pInfo.dwProcessorLevel.ToString();
            }
        }

        /// <summary>
        /// Gets the current processor's revision
        /// </summary>
        public string ProcessorRevision {
            get {
                ProcessorInfo.SystemInfo pInfo = new ProcessorInfo.SystemInfo();
                ProcessorInfo.GetSystemInfo(ref pInfo);

                return pInfo.dwProcessorRevision.ToString();
            }
        }

        /// <summary>
        /// Gets the processor's architecture
        /// </summary>
        public string ProcessorArchitecture {
            get {
                return DetermineArchitecture();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if a processor has more than one logical processor.
        /// </summary>
        /// <returns>True if there is more than one logical processor</returns>
        private bool IsMulticore() {
            if (Environment.ProcessorCount > 1) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets processor info depending on what logical processor is chosen
        /// </summary>
        /// <param name="key">Value to get from a key</param>
   //   /// <param name="specificLogicalProcessor">Which logical processor info is retrieved from. 0 represents the first core, etc.</param>
        /// <returns>The key value</returns>
        private string RetrieveProcessorInfo(string key) //int specificLogicalProcessor)
        {
            // NOTE: Remove the 0 when the functionality to retrieve info from other virtual cores is implemented
            using (RegistryKey rkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0")) // + specificLogicalProcessor);
            {
                if (!IsMulticore()) {
                    Debug.WriteLine("There is only one logical processor");
                    // specificLogicalProcessor = 0;
                }

                if (rkey != null) {
                    var obj = rkey.GetValue(key);
                    if (obj != null) {
                        return obj.ToString();
                    }
                }

                return "";
            }
        }

        /// <summary>
        /// Determines the current processor's architecture.
        /// </summary>
        /// <returns>A string representing the architecture name</returns>
        private string DetermineArchitecture() {
            ProcessorInfo.SystemInfo sysInfo = new ProcessorInfo.SystemInfo();
            ProcessorInfo.GetSystemInfo(ref sysInfo);

            switch (sysInfo.uProcessorInfo.wProcessorArchitecture) {
                case (ushort)ProcessorInfo.ProcessorArchitecture.Intel:
                    return "Intel";

                case (ushort)ProcessorInfo.ProcessorArchitecture.IA64:
                    return "Itanium (IA64)";

                case (ushort)ProcessorInfo.ProcessorArchitecture.AMD64:
                    return "AMD64";

                default:
                    return "Unknown";
            }
        }

        #endregion


        /// <summary>
        /// Class used for processor info not 
        /// conventionally able to be reached 
        /// </summary>
        private static class ProcessorInfo {
            #region P/Invokes

            [DllImport("kernel32.dll")]
            internal static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SystemInfo lpSystemInfo);

            #endregion

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

            #region Enums

            /// <summary>
            /// Processor Types, as returned by 
            /// the field dwProcessorType
            /// </summary>
            [Obsolete("dwProcessorType shouldn't be used, use wProcessorArchitecture")]
            private enum ProcessorTypes {
                PROCESSOR_INTEL_386 = 386,  // Intel 386
                PROCESSOR_INTEL_486 = 486,  // Intel 486
                PROCESSOR_INTEL_PENTIUM = 586,  // Pentium
                PROCESSOR_INTEL_IA64 = 2200, // Itanium
                PROCESSOR_AMD_X8664 = 8664, // AMD64
            };

            /// <summary>
            /// Processor Architectures
            /// </summary>
            internal enum ProcessorArchitecture : ushort {
                Intel = 0,
                IA64 = 6,
                AMD64 = 9,
                Unknown = 0xFFFF
            };

            #endregion
        }
    }
}
