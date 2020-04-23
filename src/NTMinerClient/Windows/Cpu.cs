using Microsoft.Win32;
using OpenHardwareMonitor.Hardware;
using System;
using System.Diagnostics;
using System.Management;

namespace NTMiner.Windows {
    /// <summary>
    /// Class for retrieving information related to the processor
    /// </summary>
    public sealed class Cpu {
        public static readonly Cpu Instance = new Cpu();

        private Cpu() { }

        private double _performance = 0.0f;
        public void GetSensorValue(out double performance, out float temperature, out double power) {
            performance = _performance;
            temperature = 0.0f;
            power = 0.0f;
            var computer = NTMinerContext.Computer;
            for (int i = 0; i < computer.Hardware.Length; i++) {
                var hardware = computer.Hardware[i];
                if (hardware.HardwareType == HardwareType.CPU) {
                    hardware.Update();
                    bool isCPUPackageReaded = false;
                    bool isCPUTotalReaded = false;
                    bool isPowerReaded = false;
                    for (int j = 0; j < hardware.Sensors.Length; j++) {
                        switch (hardware.Sensors[j].SensorType) {
                            case SensorType.Voltage:
                                break;
                            case SensorType.Clock:
                                break;
                            case SensorType.Temperature:
                                if (!isCPUPackageReaded) {
                                    if (hardware.Sensors[j].Name == "CPU Package") {
                                        isCPUPackageReaded = true;
                                        float? t = hardware.Sensors[j].Value;
                                        if (t.HasValue) {
                                            temperature = t.Value;
                                        }
                                    }
                                }
                                break;
                            case SensorType.Load:
                                if (!isCPUTotalReaded) {
                                    if (hardware.Sensors[j].Name == "CPU Total") {
                                        isCPUTotalReaded = true;
                                        float? t = hardware.Sensors[j].Value;
                                        if (t.HasValue) {
                                            performance = t.Value;
                                            if (performance == 0) {
                                                performance = _performance;
                                            }
                                            else {
                                                _performance = performance;
                                            }
                                        }
                                    }
                                }
                                break;
                            case SensorType.Fan:
                                break;
                            case SensorType.Flow:
                                break;
                            case SensorType.Control:
                                break;
                            case SensorType.Level:
                                break;
                            case SensorType.Factor:
                                break;
                            case SensorType.Power:
                                if (!isPowerReaded) {
                                    if (hardware.Sensors[j].Name == "CPU Package") {
                                        isPowerReaded = true;
                                        float? t = hardware.Sensors[j].Value;
                                        if (t.HasValue) {
                                            power = t.Value;
                                        }
                                    }
                                }
                                break;
                            case SensorType.Data:
                                break;
                            case SensorType.SmallData:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private static bool _isFirstGetCpuId = true;
        private static string _cpuId;
        public static string CpuId {
            get {
                if (!_isFirstGetCpuId) {
                    return _cpuId;
                }
                _isFirstGetCpuId = false;
                _cpuId = "N/A";
                try {
                    using (var searcher = new ManagementObjectSearcher("Select ProcessorID from Win32_processor"))
                    using (var query = searcher.Get()) {
                        foreach (var item in query) {
                            _cpuId = item.GetPropertyValue("ProcessorID").ToString();
                        }
                    }
                }
                catch { }
                return _cpuId;
            }
        }

        #region Properties

        /// <summary>
        /// Gets the name of the processor.
        /// </summary>
        public string Name {
            get {
                return RetrieveProcessorInfo("ProcessorNameString");
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
                SafeNativeMethods.SystemInfo pInfo = new SafeNativeMethods.SystemInfo();
                SafeNativeMethods.GetSystemInfo(ref pInfo);
                return pInfo.dwProcessorLevel.ToString();
            }
        }

        /// <summary>
        /// Gets the current processor's revision
        /// </summary>
        public string ProcessorRevision {
            get {
                SafeNativeMethods.SystemInfo pInfo = new SafeNativeMethods.SystemInfo();
                SafeNativeMethods.GetSystemInfo(ref pInfo);

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
        private string RetrieveProcessorInfo(string key) //int specificLogicalProcessor)
        {
            // NOTE: Remove the 0 when the functionality to retrieve info from other virtual cores is implemented
            using (RegistryKey rkey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0")) // + specificLogicalProcessor);
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
