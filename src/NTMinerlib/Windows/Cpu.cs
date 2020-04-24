using Microsoft.Win32;
using NTMiner.ServerNode;
using System;
using System.Diagnostics;

namespace NTMiner.Windows {
    /// <summary>
    /// Class for retrieving information related to the processor
    /// </summary>
    public sealed class Cpu {
        public static readonly Cpu Instance = new Cpu();

        private readonly PerformanceCounter _cpuCounter;
        private Cpu() {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        public CpuData ToData() {
            return new CpuData {
                ClockSpeed = this.ClockSpeed,
                Identifier = this.Identifier,
                Name = this.Name,
                NumberOfLogicalCores = this.NumberOfLogicalCores,
                ProcessorArchitecture = this.ProcessorArchitecture,
                ProcessorLevel = this.ProcessorLevel,
                VendorIdentifier = this.VendorIdentifier
            };
        }

        /// <summary>
        /// 通过PerformanceCounter获取cpu使用率
        /// </summary>
        /// <returns></returns>
        public float GetCurrentCpuUsage() {
            return _cpuCounter.NextValue();
        }

        #region Properties

        private static string _cpuName;
        /// <summary>
        /// Gets the name of the processor.
        /// </summary>
        public string Name {
            get {
                if (string.IsNullOrEmpty(_cpuName)) {
                    _cpuName = RetrieveProcessorInfo("ProcessorNameString");
                }
                return _cpuName;
            }
        }

        // This stores the total number of logical cores in the processor
        private readonly int _numberOfProcessors = Environment.ProcessorCount;
        /// <summary>
        /// Retrieves the number of logical cores on the system
        /// </summary>
        public int NumberOfLogicalCores {
            get {
                return _numberOfProcessors;
            }
        }

        private static string _clockSpeed;
        /// <summary>
        /// Gets the clock speed in MHz
        /// </summary>
        public string ClockSpeed {
            get {
                if (string.IsNullOrEmpty(_clockSpeed)) {
                    _clockSpeed = RetrieveProcessorInfo("~MHz") + " MHz";
                }
                return _clockSpeed;
            }
        }

        private static string _identifier;
        /// <summary>
        /// Gets the name of the processor.
        /// </summary>
        public string Identifier {
            get {
                if (string.IsNullOrEmpty(_identifier)) {
                    _identifier = RetrieveProcessorInfo("Identifier");
                }
                return _identifier;
            }
        }

        private static string _vendorIdentifier;
        /// <summary>
        /// Gets the vendor identifier of the processor.
        /// </summary>
        public string VendorIdentifier {
            get {
                if (string.IsNullOrEmpty(_vendorIdentifier)) {
                    _vendorIdentifier = RetrieveProcessorInfo("VendorIdentifier");
                }
                return _vendorIdentifier;
            }
        }

        private static string _processorLevel;
        /// <summary>
        /// Gets the architecture-dependent processor level. 
        /// </summary>
        public string ProcessorLevel {
            get {
                if (string.IsNullOrEmpty(_processorLevel)) {
                    SafeNativeMethods.SystemInfo pInfo = new SafeNativeMethods.SystemInfo();
                    SafeNativeMethods.GetSystemInfo(ref pInfo);
                    _processorLevel = pInfo.dwProcessorLevel.ToString();
                }
                return _processorLevel;
            }
        }

        private static string _processorRevision;
        /// <summary>
        /// Gets the current processor's revision
        /// </summary>
        public string ProcessorRevision {
            get {
                if (string.IsNullOrEmpty(_processorRevision)) {
                    SafeNativeMethods.SystemInfo pInfo = new SafeNativeMethods.SystemInfo();
                    SafeNativeMethods.GetSystemInfo(ref pInfo);

                    _processorRevision = pInfo.dwProcessorRevision.ToString();
                }
                return _processorRevision;
            }
        }

        private static string _processorArchitecture;
        /// <summary>
        /// Gets the processor's architecture
        /// </summary>
        public string ProcessorArchitecture {
            get {
                if (string.IsNullOrEmpty(_processorArchitecture)) {
                    _processorArchitecture = DetermineArchitecture();
                }
                return _processorArchitecture;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if a processor has more than one logical processor.
        /// </summary>
        /// <returns>True if there is more than one logical processor</returns>
        private bool IsMulticore() {
            if (NumberOfLogicalCores > 1) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets processor info depending on what logical processor is chosen
        /// </summary>
        /// <param name="key">Value to get from a key</param>
        /// <param name="specificLogicalProcessor">Which logical processor info is retrieved from. 0 represents the first core, etc.</param>
        /// <returns>The key value</returns>
        private string RetrieveProcessorInfo(string key) {
            // NOTE: Remove the 0 when the functionality to retrieve info from other virtual cores is implemented
            // + specificLogicalProcessor);
            using (RegistryKey rkey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0")) {
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
            SafeNativeMethods.SystemInfo sysInfo = new SafeNativeMethods.SystemInfo();
            SafeNativeMethods.GetSystemInfo(ref sysInfo);

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
