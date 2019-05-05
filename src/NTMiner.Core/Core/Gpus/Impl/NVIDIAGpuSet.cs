using NTMiner.Core.Gpus.Impl.Nvidia;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Gpus.Impl {
    internal class NVIDIAGpuSet : IGpuSet {
        #region static NvmlInit
        private static readonly string _nvsmiDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "NVIDIA Corporation", "NVSMI");
        private static bool _isNvmlInited = false;
        private static object _nvmlInitLocker = new object();
        public static bool NvmlInit() {
            if (_isNvmlInited) {
                return _isNvmlInited;
            }
            lock (_nvmlInitLocker) {
                if (_isNvmlInited) {
                    return _isNvmlInited;
                }
                if (Directory.Exists(_nvsmiDir)) {
                    try {
#if DEBUG
                        VirtualRoot.Stopwatch.Restart();
#endif
                        Windows.NativeMethods.SetDllDirectory(_nvsmiDir);
                        var nvmlReturn = NvmlNativeMethods.nvmlInit();
                        IfPrint(nvmlReturn, nameof(NvmlNativeMethods.nvmlInit));
                        _isNvmlInited = nvmlReturn == nvmlReturn.Success;
#if DEBUG
                        Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {nameof(NVIDIAGpuSet)}.{nameof(NvmlInit)}()");
#endif
                        return _isNvmlInited;
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                }
                return false;
            }
        }
        #endregion

        private static void IfPrint(nvmlReturn nvmlReturn, string nvmlMethodName) {
            if (nvmlReturn != nvmlReturn.Success && nvmlReturn != nvmlReturn.NotSupported) {
                Logger.ErrorDebugLine(nvmlMethodName + "不成功:" + nvmlReturn.ToString());
            }
        }

        private readonly Dictionary<int, IGpu> _gpus = new Dictionary<int, IGpu>() {
            {
                NTMinerRoot.GpuAllId, Gpu.GpuAll
            }
        };

        private readonly INTMinerRoot _root;
        private readonly string _driverVersion;

        public int Count {
            get {
                return _gpus.Count - 1;
            }
        }

        private readonly uint deviceCount = 0;
        public NVIDIAGpuSet(INTMinerRoot root) {
            _root = root;
            this.Properties = new List<GpuSetProperty>();
            if (Design.IsInDesignMode) {
                return;
            }
            if (NvmlInit()) {
                var nvmlReturn = NvmlNativeMethods.nvmlDeviceGetCount(ref deviceCount);
                IfPrint(nvmlReturn, nameof(NvmlNativeMethods.nvmlDeviceGetCount));
                for (int i = 0; i < deviceCount; i++) {
                    nvmlDevice nvmlDevice = new nvmlDevice();
                    nvmlReturn = NvmlNativeMethods.nvmlDeviceGetHandleByIndex((uint)i, ref nvmlDevice);
                    IfPrint(nvmlReturn, nameof(NvmlNativeMethods.nvmlDeviceGetHandleByIndex));
                    nvmlReturn = NvmlNativeMethods.nvmlDeviceGetName(nvmlDevice, out string name);
                    IfPrint(nvmlReturn, nameof(NvmlNativeMethods.nvmlDeviceGetName));
                    nvmlMemory memory = new nvmlMemory();
                    nvmlReturn = NvmlNativeMethods.nvmlDeviceGetMemoryInfo(nvmlDevice, ref memory);
                    IfPrint(nvmlReturn, nameof(NvmlNativeMethods.nvmlDeviceGetMemoryInfo));
                    // short gpu name
                    if (!string.IsNullOrEmpty(name)) {
                        name = name.Replace("GeForce GTX ", string.Empty);
                        name = name.Replace("GeForce ", string.Empty);
                    }
                    Gpu gpu = Gpu.Create(i, name);
                    gpu.TotalMemory = memory.total;
                    _gpus.Add(i, gpu);
                }
                if (deviceCount > 0) {
                    nvmlReturn = NvmlNativeMethods.nvmlSystemGetDriverVersion(out _driverVersion);
                    IfPrint(nvmlReturn, nameof(NvmlNativeMethods.nvmlSystemGetDriverVersion));
                    nvmlReturn = NvmlNativeMethods.nvmlSystemGetNVMLVersion(out string nvmlVersion);
                    IfPrint(nvmlReturn, nameof(NvmlNativeMethods.nvmlSystemGetNVMLVersion));
                    this.Properties.Add(new GpuSetProperty(GpuSetProperty.DRIVER_VERSION, "驱动版本", _driverVersion));
                    try {
                        double driverVersionNum;
                        if (double.TryParse(_driverVersion, out driverVersionNum)) {
                            var item = root.SysDicItemSet.GetSysDicItems("CudaVersion")
                                .Select(a => new { Version = double.Parse(a.Value), a })
                                .OrderByDescending(a => a.Version)
                                .FirstOrDefault(a => driverVersionNum >= a.Version);
                            if (item != null) {
                                this.Properties.Add(new GpuSetProperty("CudaVersion", "Cuda版本", item.a.Code));
                            }
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                    this.Properties.Add(new GpuSetProperty("NVMLVersion", "NVML版本", nvmlVersion));
                    Dictionary<string, string> kvs = new Dictionary<string, string> {
                            {"CUDA_DEVICE_ORDER","PCI_BUS_ID" }
                        };
                    foreach (var kv in kvs) {
                        var property = new GpuSetProperty(kv.Key, kv.Key, kv.Value);
                        this.Properties.Add(property);
                    }
                    Task.Factory.StartNew(() => {
                        foreach (var gpu in _gpus.Values) {
                            NVIDIAOverClock.RefreshGpuState(gpu);
                        }
                        // 这里会耗时5秒
                        foreach (var kv in kvs) {
                            Environment.SetEnvironmentVariable(kv.Key, kv.Value);
                        }
                    });
                }
            }
        }

        public void LoadGpuState() {
            if (NvmlInit()) {
                for (int i = 0; i < deviceCount; i++) {
                    nvmlDevice nvmlDevice = new nvmlDevice();
                    var nvmlReturn = NvmlNativeMethods.nvmlDeviceGetHandleByIndex((uint)i, ref nvmlDevice);
                    IfPrint(nvmlReturn, nameof(NvmlNativeMethods.nvmlDeviceGetHandleByIndex));
                    uint power = 0;
                    nvmlReturn = NvmlNativeMethods.nvmlDeviceGetPowerUsage(nvmlDevice, ref power);
                    IfPrint(nvmlReturn, nameof(NvmlNativeMethods.nvmlDeviceGetPowerUsage));
                    power = (uint)(power / 1000.0);
                    uint temp = 0;
                    nvmlReturn = NvmlNativeMethods.nvmlDeviceGetTemperature(nvmlDevice, nvmlTemperatureSensors.Gpu, ref temp);
                    IfPrint(nvmlReturn, nameof(NvmlNativeMethods.nvmlDeviceGetTemperature));
                    uint speed = 0;
                    nvmlReturn = NvmlNativeMethods.nvmlDeviceGetFanSpeed(nvmlDevice, ref speed);
                    IfPrint(nvmlReturn, nameof(NvmlNativeMethods.nvmlDeviceGetFanSpeed));
                    Gpu gpu = (Gpu)_gpus[i];
                    bool isChanged = gpu.Temperature != temp || gpu.PowerUsage != power || gpu.FanSpeed != speed;
                    gpu.Temperature = (int)temp;
                    gpu.PowerUsage = power;
                    gpu.FanSpeed = speed;

                    if (isChanged) {
                        VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
                    }
                }
            }
        }

        public GpuType GpuType {
            get {
                return GpuType.NVIDIA;
            }
        }

        public string DriverVersion {
            get { return _driverVersion; }
        }

        public bool TryGetGpu(int index, out IGpu gpu) {
            return _gpus.TryGetValue(index, out gpu);
        }

        public List<GpuSetProperty> Properties { get; private set; }

        public IOverClock OverClock { get; private set; } = new NVIDIAOverClock();

        public string GetProperty(string key) {
            GpuSetProperty item = this.Properties.FirstOrDefault(a => a.Code == key);
            if (item == null || item.Value == null) {
                return string.Empty;
            }
            return item.Value.ToString();
        }

        public IEnumerator<IGpu> GetEnumerator() {
            return _gpus.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _gpus.Values.GetEnumerator();
        }
    }
}
