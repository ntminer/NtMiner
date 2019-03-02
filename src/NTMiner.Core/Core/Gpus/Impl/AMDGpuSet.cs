using NTMiner.Core.Gpus.Adl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    GpuClockDelta = new GpuClockDelta(0, 0, 0, 0),
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
            string driverVersion = adlHelper.GetDriverVersion();
            this.Properties.Add(new GpuSetProperty("DriverVersion", "driver version", driverVersion));
            string[] keys = new string[]{
                "GPU_FORCE_64BIT_PTR",
                "GPU_MAX_HEAP_SIZE",
                "GPU_USE_SYNC_OBJECTS",
                "GPU_MAX_ALLOC_PERCENT",
                "GPU_SINGLE_ALLOC_PERCENT"
            };
            foreach (var key in keys) {
                this.Properties.Add(new GpuSetProperty(key, key, Environment.GetEnvironmentVariable(key)));
            }
            VirtualRoot.On<Per5SecondEvent>(
                "周期刷新显卡状态",
                LogEnum.None,
                action: message => {
                    LoadGpuStateAsync();
                });
        }

        private void LoadGpuStateAsync() {
            Task.Factory.StartNew(() => {
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
            });
        }

        public GpuType GpuType {
            get {
                return GpuType.AMD;
            }
        }

        private IGpuClockDeltaSet _gpuClockDeltaSet;
        public IGpuClockDeltaSet GpuClockDeltaSet {
            get {
                if (_gpuClockDeltaSet == null) {
                    _gpuClockDeltaSet = new AMDClockDeltaSet(_root);
                }
                return _gpuClockDeltaSet;
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
