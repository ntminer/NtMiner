using NTMiner.Core.Gpus.Adl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Gpus.Impl {
    internal class AMDGpuSet : IGpuSet {
        private readonly Dictionary<int, IGpu> _gpus = new Dictionary<int, IGpu>() {
            { Gpu.Total.Index, Gpu.Total }
        };

        private readonly INTMinerRoot _root;

        public int Count {
            get {
                return _gpus.Count - 1;
            }
        }

        private AMDGpuSet() { }

        private AdlHelper adlHelper = new AdlHelper();
        public AMDGpuSet(INTMinerRoot root) {
            _root = root;
            this.Properties = new List<GpuSetProperty>();
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

        public IGpu this[int index] {
            get {
                if (!_gpus.ContainsKey(index)) {
                    return Gpu.Total;
                }
                return _gpus[index];
            }
        }

        public List<GpuSetProperty> Properties { get; private set; }

        public IEnumerator<IGpu> GetEnumerator() {
            return _gpus.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _gpus.Values.GetEnumerator();
        }
    }
}
