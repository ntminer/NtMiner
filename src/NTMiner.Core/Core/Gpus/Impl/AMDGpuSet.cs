using NTMiner.Core.Gpus.Adl;
using System;
using System.Collections;
using System.Collections.Generic;

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
            if (NTMinerRoot.IsInDesignMode) {
                return;
            }
            adlHelper.Init();
            int deviceCount = 0;
            deviceCount = adlHelper.GpuCount;
            for (int i = 0; i < deviceCount; i++) {
                _gpus.Add(i, new Gpu {
                    Index = i,
                    Name = adlHelper.GetGpuName(i)
                });
            }
            string driverVersion = adlHelper.GetDriverVersion();
            this.Properties.Add(new GpuSetProperty("DriverVersion", "驱动版本", driverVersion));
            Global.Access<Per5SecondEvent>(Guid.Parse("7C379223-D494-4213-9659-A086FFDE36DF"), "周期刷新显卡状态", LogEnum.None, action: message => {
                LoadGpuState();
            });
        }

        private void LoadGpuState() {
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
                    Global.Happened(new GpuStateChangedEvent(gpu));
                }
            }
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
