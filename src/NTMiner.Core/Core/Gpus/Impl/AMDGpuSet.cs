using NTMiner.Core.Gpus.Impl.Amd;
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
                NTMinerRoot.GpuAllId, Gpu.GpuAll
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
#if DEBUG
            VirtualRoot.Stopwatch.Restart();
#endif
            _root = root;
            adlHelper.Init();
            this.OverClock = new AMDOverClock(adlHelper);
            int deviceCount = 0;
            deviceCount = adlHelper.GpuCount;
            for (int i = 0; i < deviceCount; i++) {
                var atiGpu = adlHelper.GetGpuName(i);
                string name = atiGpu.AdapterName;
                // short gpu name
                if (!string.IsNullOrEmpty(name)) {
                    name = name.Replace("Radeon (TM) RX ", string.Empty);
                    name = name.Replace("Radeon RX ", string.Empty);
                }
                var gpu = Gpu.Create(i, atiGpu.BusNumber.ToString(), name);
                gpu.TotalMemory = adlHelper.GetTotalMemoryByIndex(i);
                _gpus.Add(i, gpu);
            }
            if (deviceCount > 0) {
                this.DriverVersion = adlHelper.GetDriverVersion();
                this.Properties.Add(new GpuSetProperty(GpuSetProperty.DRIVER_VERSION, "驱动版本", DriverVersion));
                const ulong minG = (ulong)5 * 1024 * 1024 * 1024;
                if (_gpus.Any(a => a.Key != NTMinerRoot.GpuAllId && a.Value.TotalMemory < minG)) {
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
                        OverClock.RefreshGpuState(NTMinerRoot.GpuAllId);
                        foreach (var kv in kvs) {
                            Environment.SetEnvironmentVariable(kv.Key, kv.Value);
                        }
                    });
                }
            }
#if DEBUG
            Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
        }

        public void LoadGpuState() {
#if DEBUG
            VirtualRoot.Stopwatch.Restart();
#endif
            for (int i = 0; i < Count; i++) {
                uint power = adlHelper.GetPowerUsageByIndex(i);
                int temp = adlHelper.GetTemperatureByIndex(i);
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
#if DEBUG
            Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.{nameof(LoadGpuState)}");
#endif
        }

        public GpuType GpuType {
            get {
                return GpuType.AMD;
            }
        }

        public string DriverVersion { get; private set; }

        public bool TryGetGpu(int index, out IGpu gpu) {
            return _gpus.TryGetValue(index, out gpu);
        }

        public List<GpuSetProperty> Properties { get; private set; }

        public IOverClock OverClock { get; private set; }

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
