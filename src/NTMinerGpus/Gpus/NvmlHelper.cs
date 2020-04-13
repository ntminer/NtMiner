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
                return $"GpuIndex={GpuIndex.ToString()},BusId={BusId.ToString()},Name={Name},TotalMemory={TotalMemory.ToString()}";
            }
        }

        #region static NvmlInit
        private static readonly string _nvsmiDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "NVIDIA Corporation", "NVSMI");
        private static readonly string _nvmlDllFileFullName = Path.Combine(_nvsmiDir, "nvml.dll");
        private static readonly string _nvmlDllFileFullName2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "nvml.dll");
        private static bool _isNvmlInited = false;
        private static readonly object _locker = new object();
        private static int _nvmlInitFailCount = 0;
        private static bool NvmlInit() {
            if (_isNvmlInited) {
                return _isNvmlInited;
            }
            lock (_locker) {
                if (_isNvmlInited) {
                    return _isNvmlInited;
                }
                try {
#if DEBUG
                    NTStopwatch.Start();
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
                    // 没什么用，做个防御
                    if (!_isNvmlInited) {
                        _nvmlInitFailCount++;
                        if (_nvmlInitFailCount >= 10) {
                            _isNvmlInited = true;
                        }
                    }
#if DEBUG
                    var elapsedMilliseconds = NTStopwatch.Stop();
                    if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                        Write.DevTimeSpan($"耗时{elapsedMilliseconds} {nameof(NvmlHelper)}.{nameof(NvmlInit)}()");
                    }
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
                        Write.DevError(() => $"{nameof(NvmlNativeMethods.nvmlDeviceGetCount)} {r.ToString()}");
                    }
                    for (int i = 0; i < deviceCount; i++) {
                        NvGpu gpu = new NvGpu {
                            GpuIndex = i
                        };
                        nvmlDevice nvmlDevice = new nvmlDevice();
                        r = NvmlNativeMethods.nvmlDeviceGetHandleByIndex((uint)i, ref nvmlDevice);
                        _nvmlDevices.Add(nvmlDevice);
                        if (r != nvmlReturn.Success) {
                            Write.DevError(() => $"{nameof(NvmlNativeMethods.nvmlDeviceGetHandleByIndex)}({((uint)i).ToString()}) {r.ToString()}");
                        }
                        r = NvmlNativeMethods.nvmlDeviceGetName(nvmlDevice, out string name);
                        if (r != nvmlReturn.Success) {
                            Write.DevError(() => $"{nameof(NvmlNativeMethods.nvmlDeviceGetName)} {r.ToString()}");
                        }
                        nvmlMemory memory = new nvmlMemory();
                        r = NvmlNativeMethods.nvmlDeviceGetMemoryInfo(nvmlDevice, ref memory);
                        if (r != nvmlReturn.Success) {
                            Write.DevError(() => $"{nameof(NvmlNativeMethods.nvmlDeviceGetMemoryInfo)} {r.ToString()}");
                        }
                        // short gpu name
                        if (!string.IsNullOrEmpty(name)) {
                            name = name.Replace("GeForce GTX ", string.Empty);
                            name = name.Replace("GeForce ", string.Empty);
                        }
                        nvmlPciInfo pci = new nvmlPciInfo();
                        r = NvmlNativeMethods.nvmlDeviceGetPciInfo(nvmlDevice, ref pci);
                        if (r != nvmlReturn.Success) {
                            Write.DevError(() => $"{nameof(NvmlNativeMethods.nvmlDeviceGetPciInfo)} {r.ToString()}");
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

        private readonly HashSet<int> _nvmlDeviceGetPowerUsageNotSupporteds = new HashSet<int>();
        public uint GetPowerUsage(int gpuIndex) {
            if (!NvmlInit() || !TryGetNvmlDevice(gpuIndex, out nvmlDevice nvmlDevice)) {
                return 0;
            }
            if (_nvmlDeviceGetPowerUsageNotSupporteds.Contains(gpuIndex)) {
                return 0;
            }
            uint power = 0;
            try {
                var r = NvmlNativeMethods.nvmlDeviceGetPowerUsage(nvmlDevice, ref power);
                power = (uint)(power / 1000.0);
                if (r != nvmlReturn.Success) {
                    if (r == nvmlReturn.NotSupported) {
                        _nvmlDeviceGetPowerUsageNotSupporteds.Add(gpuIndex);
                    }
                    Write.DevError(() => $"{nameof(NvmlNativeMethods.nvmlDeviceGetPowerUsage)} {r.ToString()}");
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
                    Write.DevError(() => $"{nameof(NvmlNativeMethods.nvmlDeviceGetTemperature)} {r.ToString()}");
                }
            }
            catch {
            }
            return temp;
        }

        private readonly HashSet<int> _nvmlDeviceGetFanSpeedNotSupporteds = new HashSet<int>();
        public uint GetFanSpeed(int gpuIndex) {
            if (!NvmlInit() || !TryGetNvmlDevice(gpuIndex, out nvmlDevice nvmlDevice)) {
                return 0;
            }
            if (_nvmlDeviceGetFanSpeedNotSupporteds.Contains(gpuIndex)) {
                return 0;
            }
            uint fanSpeed = 0;
            try {
                var r = NvmlNativeMethods.nvmlDeviceGetFanSpeed(nvmlDevice, ref fanSpeed);
                if (r != nvmlReturn.Success) {
                    if (r == nvmlReturn.NotSupported) {
                        _nvmlDeviceGetFanSpeedNotSupporteds.Add(gpuIndex);
                    }
                    Write.DevError(() => $"{nameof(NvmlNativeMethods.nvmlDeviceGetFanSpeed)} {r.ToString()}");
                }
            }
            catch {
            }
            return fanSpeed;
        }

        public void GetVersion(out Version driverVersion, out string nvmlVersion) {
            driverVersion = new Version();
            nvmlVersion = "0.0";
            if (!NvmlInit()) {
                return;
            }
            try {
                var r = NvmlNativeMethods.nvmlSystemGetDriverVersion(out string version);
                if (r != nvmlReturn.Success) {
                    Write.DevError(() => $"{nameof(NvmlNativeMethods.nvmlSystemGetDriverVersion)} {r.ToString()}");
                }
                r = NvmlNativeMethods.nvmlSystemGetNVMLVersion(out nvmlVersion);
                if (r != nvmlReturn.Success) {
                    Write.DevError(() => $"{nameof(NvmlNativeMethods.nvmlSystemGetNVMLVersion)} {r.ToString()}");
                }
                if (!string.IsNullOrEmpty(version) && Version.TryParse(version, out Version v)) {
                    driverVersion = v;
                }
                if (string.IsNullOrEmpty(nvmlVersion)) {
                    nvmlVersion = "0.0";
                }
            }
            catch {
            }
        }
    }
}
