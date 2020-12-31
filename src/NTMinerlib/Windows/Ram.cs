using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NTMiner.Windows {
    public sealed class Ram {
        public static Ram Instance { get; private set; } = new Ram();

        #region Properties

        private PerformanceCounter _ramCounterProcess;
        private bool _firstRamCounterProcess = true;
        private readonly object _locker = new object();
        private PerformanceCounter RamCounterProcess {
            get {
                if (_ramCounterProcess == null && _firstRamCounterProcess) {
                    _firstRamCounterProcess = false;
                    lock (_locker) {
                        if (_ramCounterProcess == null) {
                            try {
                                // 进程占用的物理内存的大小。由于包含共享内存部分和其他资源，所以其实并不准；但这个值就是在任务管理器中看到的值。
                                _ramCounterProcess = new PerformanceCounter("Processor", "Working Set", VirtualRoot.ProcessName);
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e);
                                _ramCounterProcess = null;
                            }
                        }
                    }
                }
                return _ramCounterProcess;
            }
        }

        public float GetTotalCpuUsage() {
            if (RamCounterProcess == null) {
                return 0.0f;
            }
            return RamCounterProcess.NextValue();
        }

        private ulong _totalPhysicalMemory = ulong.MinValue;
        /// <summary>
        /// Gets the total physical memory in bytes
        /// </summary>
        public ulong TotalPhysicalMemory {
            get {
                if (_totalPhysicalMemory != ulong.MinValue) {
                    return _totalPhysicalMemory;
                }
                MemoryStatusEx mEx = new MemoryStatusEx();
                if (SafeNativeMethods.GlobalMemoryStatusEx(mEx)) {
                    _totalPhysicalMemory = mEx.ullTotalPhys;
                    if (_totalPhysicalMemory == ulong.MinValue) {
                        _totalPhysicalMemory = 0;
                    }
                }
                else {
                    _totalPhysicalMemory = 0;
                }

                return _totalPhysicalMemory;
            }
        }

        /// <summary>
        /// Gets the available physical memory in bytes
        /// </summary>
        public ulong AvailablePhysicalMemory {
            get {
                MemoryStatusEx mEx = new MemoryStatusEx();
                if (SafeNativeMethods.GlobalMemoryStatusEx(mEx)) {
                    return mEx.ullAvailPhys;
                }

                return 0;
            }
        }

        #endregion

        #region P/Invoke Related

        /// <summary>
        /// Class that represents the C/C++ structure MEMORYSTATUSEX
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class MemoryStatusEx {
            /// <summary>
            /// The size of the structure, in bytes.
            /// </summary>
            internal uint dwLength;

            /// <summary>
            /// A number between 0 and 100 that specifies the approximate percentage of physical memory 
            /// that is in use (0 indicates no memory use and 100 indicates full memory use).
            /// </summary>
            internal uint dwMemoryLoad;

            /// <summary>
            /// The amount of actual physical memory, in bytes.
            /// </summary>
            internal ulong ullTotalPhys;

            /// <summary>
            /// The amount of physical memory currently available, in bytes.
            /// 
            /// This is the amount of physical memory that can be immediately 
            /// reused without having to write its contents to disk first. 
            /// 
            /// It is the sum of the size of the standby, free, and zero lists.
            /// </summary>
            internal ulong ullAvailPhys;

            /// <summary>
            /// The current committed memory limit for the system or the 
            /// current process, whichever is smaller, in bytes.
            /// </summary>
            internal ulong ullTotalPageFile;

            /// <summary>
            /// The maximum amount of memory the current process can commit, in bytes.
            /// </summary>
            internal ulong ullAvailPageFile;

            /// <summary>
            /// The size of the user-mode portion of the virtual 
            /// address space of the calling process, in bytes. 
            /// </summary>
            internal ulong ullTotalVirtual;

            /// <summary>
            /// The amount of unreserved and uncommitted memory currently in the user-mode 
            /// portion of the virtual address space of the calling process, in bytes.
            /// </summary>
            internal ulong ullAvailVirtual;

            /// <summary>
            /// Reserved. This value is always 0.
            /// </summary>
            internal ulong ullAvailExtendedVirtual;

            /// <summary>
            /// Constructor
            /// </summary>
            internal MemoryStatusEx() {
                this.dwLength = (uint)Marshal.SizeOf(this);
            }
        }

        #endregion
    }
}
