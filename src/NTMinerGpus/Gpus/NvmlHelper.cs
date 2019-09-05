using NTMiner.Gpus.Nvml;
using System;
using System.Collections.Generic;
using System.IO;

namespace NTMiner.Gpus {
    public class NvmlHelper {
        public class NvGpu {
            public static readonly NvGpu Empty = new NvGpu {
                GpuIndex = -1,
                BusId = -1,
                Name = string.Empty,
                TotalMemory = 0
            };

            public int GpuIndex { get; set; }
            public int BusId { get; set; }
            public string Name { get; set; }

            public ulong TotalMemory { get; set; }

            public override string ToString() {
                return $"GpuIndex={GpuIndex},BusId={BusId},Name={Name},TotalMemory={TotalMemory}";
            }
        }

        #region static NvmlInit
        private static readonly string _nvsmiDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "NVIDIA Corporation", "NVSMI");
        private static readonly string _nvmlDllFileFullName = Path.Combine(_nvsmiDir, "nvml.dll");
        private static readonly string _nvmlDllFileFullName2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "nvml.dll");
        private static bool _isNvmlInited = false;
        private static readonly object _nvmlInitLocker = new object();
        private static bool NvmlInit() {
            if (_isNvmlInited) {
                return _isNvmlInited;
            }
            lock (_nvmlInitLocker) {
                if (_isNvmlInited) {
                    return _isNvmlInited;
                }
                try {
#if DEBUG
                    Write.Stopwatch.Restart();
#endif
                    if (!Directory.Exists(_nvsmiDir)) {
                        Directory.CreateDirectory(_nvsmiDir);
                    }
                    if (!File.Exists(_nvmlDllFileFullName) && File.Exists(_nvmlDllFileFullName2)) {
                        File.Copy(_nvmlDllFileFullName2, _nvmlDllFileFullName);
                    }
                    NvmlNativeMethods.SetDllDirectory(_nvsmiDir);
                    var nvmlReturn = NvmlNativeMethods.nvmlInit();
                    NvmlNativeMethods.SetDllDirectory(null);
                    _isNvmlInited = nvmlReturn == nvmlReturn.Success;
#if DEBUG
                        Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {nameof(NvmlHelper)}.{nameof(NvmlInit)}()");
#endif
                    return _isNvmlInited;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                return false;
            }
        }
        #endregion

        private readonly List<nvmlDevice> _nvmlDevices = new List<nvmlDevice>();

        public NvmlHelper() { }

        public List<NvGpu> GetGpus() {
            List<NvGpu> results = new List<NvGpu>();
            try {
                if (NvmlInit()) {
                    _nvmlDevices.Clear();
                    uint deviceCount = 0;
                    var r = NvmlNativeMethods.nvmlDeviceGetCount(ref deviceCount);
                    if (r != nvmlReturn.Success) {
                        Write.DevError($"{nameof(NvmlNativeMethods.nvmlDeviceGetCount)} {r}");
                    }
                    for (int i = 0; i < deviceCount; i++) {
                        NvGpu gpu = new NvGpu {
                            GpuIndex = i
                        };
                        nvmlDevice nvmlDevice = new nvmlDevice();
                        r = NvmlNativeMethods.nvmlDeviceGetHandleByIndex((uint)i, ref nvmlDevice);
                        _nvmlDevices.Add(nvmlDevice);
                        if (r != nvmlReturn.Success) {
                            Write.DevError($"{nameof(NvmlNativeMethods.nvmlDeviceGetHandleByIndex)}({(uint)i}) {r}");
                        }
                        r = NvmlNativeMethods.nvmlDeviceGetName(nvmlDevice, out string name);
                        if (r != nvmlReturn.Success) {
                            Write.DevError($"{nameof(NvmlNativeMethods.nvmlDeviceGetName)} {r}");
                        }
                        nvmlMemory memory = new nvmlMemory();
                        r = NvmlNativeMethods.nvmlDeviceGetMemoryInfo(nvmlDevice, ref memory);
                        if (r != nvmlReturn.Success) {
                            Write.DevError($"{nameof(NvmlNativeMethods.nvmlDeviceGetMemoryInfo)} {r}");
                        }
                        // short gpu name
                        if (!string.IsNullOrEmpty(name)) {
                            name = name.Replace("GeForce GTX ", string.Empty);
                            name = name.Replace("GeForce ", string.Empty);
                        }
                        nvmlPciInfo pci = new nvmlPciInfo();
                        r = NvmlNativeMethods.nvmlDeviceGetPciInfo(nvmlDevice, ref pci);
                        if (r != nvmlReturn.Success) {
                            Write.DevError($"{nameof(NvmlNativeMethods.nvmlDeviceGetPciInfo)} {r}");
                        }
                        gpu.Name = name;
                        gpu.BusId = (int)pci.bus;
                        gpu.TotalMemory = memory.total;
                        results.Add(gpu);
                    }
                }
            }
            catch {
            }

            return results;
        }

        private bool TryGetNvmlDevice(int gpuIndex, out nvmlDevice nvmlDevice) {
            nvmlDevice = default;
            if (gpuIndex < 0 || gpuIndex >= _nvmlDevices.Count) {
                return false;
            }
            nvmlDevice = _nvmlDevices[gpuIndex];
            return true;
        }

        public uint GetPowerUsage(int gpuIndex) {
            if (!NvmlInit() || !TryGetNvmlDevice(gpuIndex, out nvmlDevice nvmlDevice)) {
                return 0;
            }
            uint power = 0;
            try {
                var r = NvmlNativeMethods.nvmlDeviceGetPowerUsage(nvmlDevice, ref power);
                power = (uint)(power / 1000.0);
                if (r != nvmlReturn.Success) {
                    Write.DevError($"{nameof(NvmlNativeMethods.nvmlDeviceGetPowerUsage)} {r}");
                }
            }
            catch {
            }
            return power;
        }

        public uint GetTemperature(int gpuIndex) {
            if (!NvmlInit() || !TryGetNvmlDevice(gpuIndex, out nvmlDevice nvmlDevice)) {
                return 0;
            }
            uint temp = 0;
            try {
                var r = NvmlNativeMethods.nvmlDeviceGetTemperature(nvmlDevice, nvmlTemperatureSensors.Gpu, ref temp);
                if (r != nvmlReturn.Success) {
                    Write.DevError($"{nameof(NvmlNativeMethods.nvmlDeviceGetTemperature)} {r}");
                }
            }
            catch {
            }
            return temp;
        }

        public uint GetFanSpeed(int gpuIndex) {
            if (!NvmlInit() || !TryGetNvmlDevice(gpuIndex, out nvmlDevice nvmlDevice)) {
                return 0;
            }
            uint fanSpeed = 0;
            try {
                var r = NvmlNativeMethods.nvmlDeviceGetFanSpeed(nvmlDevice, ref fanSpeed);
                if (r != nvmlReturn.Success) {
                    Write.DevError($"{nameof(NvmlNativeMethods.nvmlDeviceGetFanSpeed)} {r}");
                }
            }
            catch {
            }
            return fanSpeed;
        }

        public void GetVersion(out string driverVersion, out string nvmlVersion) {
            driverVersion = string.Empty;
            nvmlVersion = string.Empty;
            if (!NvmlInit()) {
                return;
            }
            try {
                var r = NvmlNativeMethods.nvmlSystemGetDriverVersion(out driverVersion);
                if (r != nvmlReturn.Success) {
                    Write.DevError($"{nameof(NvmlNativeMethods.nvmlSystemGetDriverVersion)} {r}");
                }
                r = NvmlNativeMethods.nvmlSystemGetNVMLVersion(out nvmlVersion);
                if (r != nvmlReturn.Success) {
                    Write.DevError($"{nameof(NvmlNativeMethods.nvmlSystemGetNVMLVersion)} {r}");
                }
            }
            catch {
            }
        }
    }
}
