using NTMiner.Core.Gpus.Nvml;
using NTMiner.MinerClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Gpus.Impl {
    internal class NVIDIAGpuSet : IGpuSet {
        private readonly Dictionary<int, IGpu> _gpus = new Dictionary<int, IGpu>() {
            {
                NTMinerRoot.GpuAllId, new Gpu{
                    Index = NTMinerRoot.GpuAllId,
                    Name = "全部显卡",
                    Temperature = 0,
                    FanSpeed = 0,
                    PowerUsage = 0,
                    CoreClockDelta = 0,
                    MemoryClockDelta = 0,
                    OverClock = new NVIDIAOverClock()
                }
            }
        };

        private readonly INTMinerRoot _root;

        public int Count {
            get {
                return _gpus.Count - 1;
            }
        }

        private NVIDIAGpuSet() {
            this.Properties = new List<GpuSetProperty>();
        }

        private readonly uint deviceCount = 0;
        public NVIDIAGpuSet(INTMinerRoot root) : this() {
            _root = root;
            if (Design.IsInDesignMode) {
                return;
            }
            string nvsmiDir = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles), "NVIDIA Corporation", "NVSMI");
            if (Directory.Exists(nvsmiDir)) {
                Windows.NativeMethods.SetDllDirectory(nvsmiDir);
                NvmlNativeMethods.nvmlInit();
                NvmlNativeMethods.nvmlDeviceGetCount(ref deviceCount);
                for (int i = 0; i < deviceCount; i++) {
                    nvmlDevice nvmlDevice = new nvmlDevice();
                    NvmlNativeMethods.nvmlDeviceGetHandleByIndex((uint)i, ref nvmlDevice);
                    uint gClock = 0, mClock = 0;
                    NvmlNativeMethods.nvmlDeviceGetName(nvmlDevice, out string name);
                    NvmlNativeMethods.nvmlDeviceGetMaxClockInfo(nvmlDevice, nvmlClockType.Graphics, ref gClock);
                    NvmlNativeMethods.nvmlDeviceGetMaxClockInfo(nvmlDevice, nvmlClockType.Mem, ref mClock);
                    if (!string.IsNullOrEmpty(name)) {
                        name = name.Replace("GeForce ", string.Empty);
                    }
                    Gpu gpu = new Gpu {
                        Index = i,
                        Name = name,
                        Temperature = 0,
                        PowerUsage = 0,
                        FanSpeed = 0,
                        OverClock = new NVIDIAOverClock()
                    };
                    _gpus.Add(i, gpu);
                }
                if (deviceCount > 0) {
                    NvmlNativeMethods.nvmlSystemGetDriverVersion(out string driverVersion);
                    NvmlNativeMethods.nvmlSystemGetNVMLVersion(out string nvmlVersion);
                    this.Properties.Add(new GpuSetProperty("DriverVersion", "driver version", driverVersion));
                    this.Properties.Add(new GpuSetProperty("NVMLVersion", "NVML version", nvmlVersion));
                    Dictionary<string, string> kvs = new Dictionary<string, string> {
                        {"CUDA_DEVICE_ORDER","PCI_BUS_ID" }
                    };
                    foreach (var kv in kvs) {
                        var property = new GpuSetProperty(kv.Key, kv.Key, kv.Value);
                        this.Properties.Add(property);
                    }
                    Task.Factory.StartNew(() => {
                        // 这里会耗时5秒
                        foreach (var kv in kvs) {
                            Environment.SetEnvironmentVariable(kv.Key, kv.Value);
                        }
                        foreach (var gpu in _gpus.Values) {
                            NVIDIAOverClock.RefreshGpuState(gpu);
                        }
                    });
                }
            }
        }

        ~NVIDIAGpuSet() {
            NvmlNativeMethods.nvmlShutdown();
        }

        public void LoadGpuState() {
            for (int i = 0; i < deviceCount; i++) {
                nvmlDevice nvmlDevice = new nvmlDevice();
                NvmlNativeMethods.nvmlDeviceGetHandleByIndex((uint)i, ref nvmlDevice);
                uint power = 0;
                NvmlNativeMethods.nvmlDeviceGetPowerUsage(nvmlDevice, ref power);
                power = (uint)(power / 1000.0);
                uint temp = 0;
                NvmlNativeMethods.nvmlDeviceGetTemperature(nvmlDevice, nvmlTemperatureSensors.Gpu, ref temp);
                uint speed = 0;
                NvmlNativeMethods.nvmlDeviceGetFanSpeed(nvmlDevice, ref speed);

                Gpu gpu = (Gpu)_gpus[i];
                bool isChanged = gpu.Temperature != temp || gpu.PowerUsage != power || gpu.FanSpeed != speed;
                gpu.Temperature = temp;
                gpu.PowerUsage = power;
                gpu.FanSpeed = speed;

                if (isChanged) {
                    VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
                }
            }
        }

        public GpuType GpuType {
            get {
                return GpuType.NVIDIA;
            }
        }

        public bool TryGetGpu(int index, out IGpu gpu) {
            return _gpus.TryGetValue(index, out gpu);
        }

        public List<GpuSetProperty> Properties { get; private set; }

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
