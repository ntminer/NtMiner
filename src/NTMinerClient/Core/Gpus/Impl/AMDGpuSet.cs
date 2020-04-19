using NTMiner.Gpus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Gpus.Impl {
    internal class AMDGpuSet : IGpuSet {
        private readonly Dictionary<int, Gpu> _gpus = new Dictionary<int, Gpu>() {
            {
                NTMinerContext.GpuAllId, Gpu.GpuAll
            }
        };

        public int Count {
            get {
                return _gpus.Count - 1;
            }
        }

        private readonly AdlHelper adlHelper = new AdlHelper();
        public AMDGpuSet() {
#if DEBUG
            NTStopwatch.Start();
#endif
            adlHelper.Init();
            this.Properties = new List<GpuSetProperty>();
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
                var gpu = Gpu.Create(GpuType.AMD, i, atiGpu.BusNumber.ToString(), name);
                gpu.TotalMemory = adlHelper.GetTotalMemory(i);
                _gpus.Add(i, gpu);
            }
            if (deviceCount > 0) {
                this.DriverVersion = adlHelper.GetDriverVersion();
                this.Properties.Add(new GpuSetProperty(GpuSetProperty.DRIVER_VERSION, "驱动版本", DriverVersion));
                const ulong minG = (ulong)5 * 1024 * 1024 * 1024;
                bool has470 = _gpus.Any(a => a.Key != NTMinerContext.GpuAllId && a.Value.TotalMemory < minG);
                if (has470) {
                    Dictionary<string, string> kvs = new Dictionary<string, string> {
                        {"GPU_MAX_ALLOC_PERCENT","100" },
                        {"GPU_MAX_HEAP_SIZE","100" },
                        {"GPU_SINGLE_ALLOC_PERCENT","100" }
                    };
                    foreach (var kv in kvs) {
                        var property = new GpuSetProperty(kv.Key, kv.Key, kv.Value);
                        this.Properties.Add(property);
                    }
                    Task.Factory.StartNew(() => {
                        OverClock.RefreshGpuState(NTMinerContext.GpuAllId);
                        foreach (var kv in kvs) {
                            Environment.SetEnvironmentVariable(kv.Key, kv.Value);
                        }
                    });
                }
                else {
                    Task.Factory.StartNew(() => {
                        OverClock.RefreshGpuState(NTMinerContext.GpuAllId);
                    });
                }
            }
#if DEBUG
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
            }
#endif
        }

        public void LoadGpuState() {
#if DEBUG
            NTStopwatch.Start();
#endif
            for (int i = 0; i < Count; i++) {
                LoadGpuState(i);
            }
#if DEBUG
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.{nameof(LoadGpuState)}");
            }
#endif
        }

        public void LoadGpuState(int gpuIndex) {
            if (gpuIndex == NTMinerContext.GpuAllId) {
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
                VirtualRoot.RaiseEvent(new GpuStateChangedEvent(Guid.Empty, gpu));
            }
        }

        public GpuType GpuType {
            get {
                return GpuType.AMD;
            }
        }

        public Version DriverVersion { get; private set; }

        public bool TryGetGpu(int index, out IGpu gpu) {
            var r = _gpus.TryGetValue(index, out Gpu temp);
            gpu = temp;
            return r;
        }

        public List<GpuSetProperty> Properties { get; private set; }

        public IOverClock OverClock { get; private set; }

        public IEnumerable<IGpu> AsEnumerable() {
            return _gpus.Values;
        }
    }
}
