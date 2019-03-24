using NTMiner.Core.Gpus.Adl;
using NTMiner.MinerClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace NTMiner.Core.Gpus.Impl {
    internal class AMDGpuSet : IGpuSet {
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
                    OverClock = new AMDOverClock()
                }
            }
        };

        private readonly INTMinerRoot _root;

        public int Count {
            get {
                return _gpus.Count - 1;
            }
        }

        private AMDGpuSet() {
            this.Properties = new List<GpuSetProperty>();
        }

        private AdlHelper adlHelper = new AdlHelper();
        public AMDGpuSet(INTMinerRoot root) : this() {
            _root = root;
            if (Design.IsInDesignMode) {
                return;
            }
            adlHelper.Init();
            int deviceCount = 0;
            deviceCount = adlHelper.GpuCount;
            for (int i = 0; i < deviceCount; i++) {
                _gpus.Add(i, new Gpu {
                    Index = i,
                    Name = adlHelper.GetGpuName(i),
                    Temperature = 0,
                    PowerUsage = 0,
                    FanSpeed = 0,
                    OverClock = new AMDOverClock()
                });
            }
            if (deviceCount > 0) {
                this.Properties.Add(new GpuSetProperty("DriverVersion", "driver version", GetDriverVersion()));
                Dictionary<string, string> kvs = new Dictionary<string, string> {
                    {"GPU_FORCE_64BIT_PTR","0" },
                    {"GPU_MAX_ALLOC_PERCENT","100" },
                    {"GPU_MAX_HEAP_SIZE","100" },
                    {"GPU_SINGLE_ALLOC_PERCENT","100" },
                    { "GPU_USE_SYNC_OBJECTS","1" }
                };
                foreach (var kv in kvs) {
                    var property = new GpuSetProperty(kv.Key, kv.Key, kv.Value);
                    this.Properties.Add(property);
                }
                Task.Factory.StartNew(() => {
                    foreach (var kv in kvs) {
                        Environment.SetEnvironmentVariable(kv.Key, kv.Value);
                    }
                });
            }
        }

        public string GetDriverVersion() {
            try {
                ManagementObjectSearcher videos = new ManagementObjectSearcher("select DriverVersion from Win32_VideoController");
                foreach (var obj in videos.Get()) {
                    return obj["DriverVersion"] ?.ToString();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
            return "0.0";
        }

        public void LoadGpuState() {
            for (int i = 0; i < Count; i++) {
                uint power = adlHelper.GetPowerUsageByIndex(i);
                uint temp = adlHelper.GetTemperatureByIndex(i);
                uint speed = adlHelper.GetFanSpeedByIndex(i);

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
                return GpuType.AMD;
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
