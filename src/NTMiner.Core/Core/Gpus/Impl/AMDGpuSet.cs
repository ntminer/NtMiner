using NTMiner.Gpus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Gpus.Impl {
    internal class AMDGpuSet : IGpuSet {
        private readonly Dictionary<int, Gpu> _gpus = new Dictionary<int, Gpu>() {
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

        private readonly AdlHelper adlHelper = new AdlHelper();
        public AMDGpuSet(INTMinerRoot root) : this() {
#if DEBUG
            Write.Stopwatch.Restart();
#endif
            _root = root;
            adlHelper.Init();
            this.OverClock = new GpuOverClock(adlHelper);
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
                gpu.TotalMemory = adlHelper.GetTotalMemory(i);
                _gpus.Add(i, gpu);
            }
            if (deviceCount > 0) {
                this.DriverVersion = adlHelper.GetDriverVersion();
                this.Properties.Add(new GpuSetProperty(GpuSetProperty.DRIVER_VERSION, "驱动版本", DriverVersion));
                const ulong minG = (ulong)5 * 1024 * 1024 * 1024;
                bool has470 = _gpus.Any(a => a.Key != NTMinerRoot.GpuAllId && a.Value.TotalMemory < minG);
                if (has470) {
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
                else {
                    Task.Factory.StartNew(() => {
                        OverClock.RefreshGpuState(NTMinerRoot.GpuAllId);
                    });
                }
            }
#if DEBUG
            Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
        }

        public void LoadGpuState() {
#if DEBUG
            Write.Stopwatch.Restart();
#endif
            for (int i = 0; i < Count; i++) {
                LoadGpuState(i);
            }
#if DEBUG
            Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.{nameof(LoadGpuState)}");
#endif
        }

        public void LoadGpuState(int gpuIndex) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                return;
            }
            uint power = adlHelper.GetPowerUsage(gpuIndex);
            int temp = adlHelper.GetTemperature(gpuIndex);
            uint speed = adlHelper.GetFanSpeed(gpuIndex);

            Gpu gpu = _gpus[gpuIndex];
            bool isChanged = gpu.Temperature != temp || gpu.PowerUsage != power || gpu.FanSpeed != speed;
            gpu.Temperature = temp;
            gpu.PowerUsage = power;
            gpu.FanSpeed = speed;

            if (isChanged) {
                VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
            }
        }

        public GpuType GpuType {
            get {
                return GpuType.AMD;
            }
        }

        public string DriverVersion { get; private set; }

        public bool TryGetGpu(int index, out IGpu gpu) {
            Gpu temp;
            var r = _gpus.TryGetValue(index, out temp);
            gpu = temp;
            return r;
        }

        public List<GpuSetProperty> Properties { get; private set; }

        public IOverClock OverClock { get; private set; }

        public IEnumerator<IGpu> GetEnumerator() {
            return _gpus.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _gpus.Values.GetEnumerator();
        }
    }
}
