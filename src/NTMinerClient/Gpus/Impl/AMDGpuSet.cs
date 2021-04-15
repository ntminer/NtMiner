using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Gpus.Impl {
    internal class AMDGpuSet : IGpuSet {
        private readonly Dictionary<int, Gpu> _gpus = new Dictionary<int, Gpu>() {
            {
                NTMinerContext.GpuAllId, Gpu.GpuAll
            }
        };
        private readonly Version _driverVersion = new Version();

        public int Count {
            get {
                return _gpus.Count - 1;
            }
        }

        private readonly AdlHelper _adlHelper = new AdlHelper();
        public AMDGpuSet() {
#if DEBUG
            NTStopwatch.Start();
#endif
            this.Properties = new List<GpuSetProperty>();
            this.OverClock = new GpuOverClock(_adlHelper);
            VirtualRoot.BuildEventPath<AppExitEvent>("程序退出时调用adlHelper.Close", LogEnum.None, this.GetType(), PathPriority.Normal, message => {
                _adlHelper.Close();
            });
            if (_adlHelper.ATIGpus.Count > 0) {
                int i = 0;
                foreach (var atiGpu in _adlHelper.ATIGpus) {
                    string name = atiGpu.AdapterName;
                    // short gpu name
                    if (!string.IsNullOrEmpty(name)) {
                        name = name.Replace("Radeon (TM) RX ", string.Empty);
                        name = name.Replace("Radeon RX ", string.Empty);
                    }
                    var gpu = Gpu.Create(GpuType.AMD, i, atiGpu.BusNumber.ToString(), name);
                    gpu.MemoryTimingLevels = atiGpu.MemoryTimingLevels;
                    gpu.CurrentMemoryTimingLevel = atiGpu.CurrentMemoryTimingLevel;
                    gpu.TotalMemory = _adlHelper.GetTotalMemory(i);
                    _gpus.Add(i, gpu);
                    i++;
                }
                this._driverVersion = _adlHelper.GetDriverVersion();
                this.Properties.Add(new GpuSetProperty(GpuSetProperty.DRIVER_VERSION, "驱动版本", DriverVersion));
                const ulong minG = 5 * NTKeyword.ULongG;
                bool has470 = _gpus.Any(a => a.Key != NTMinerContext.GpuAllId && a.Value.TotalMemory < minG);
                if (has470) {
                    Dictionary<string, string> kvs = new Dictionary<string, string> {
                        ["GPU_MAX_ALLOC_PERCENT"] = "100",
                        ["GPU_MAX_HEAP_SIZE"] = "100",
                        ["GPU_SINGLE_ALLOC_PERCENT"] = "100"
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
                NTMinerConsole.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
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
                NTMinerConsole.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.{nameof(LoadGpuState)}");
            }
#endif
        }

        public void LoadGpuState(int gpuIndex) {
            if (gpuIndex == NTMinerContext.GpuAllId) {
                return;
            }
            _adlHelper.GetPowerFanTemp(gpuIndex, out uint power, out uint fanSpeed, out int coreTemp, out int memTemp);

            Gpu gpu = _gpus[gpuIndex];
            bool isChanged = gpu.Temperature != coreTemp || gpu.MemoryTemperature != memTemp || gpu.PowerUsage != power || gpu.FanSpeed != fanSpeed;
            gpu.Temperature = coreTemp;
            gpu.MemoryTemperature = memTemp;
            gpu.PowerUsage = power;
            gpu.FanSpeed = fanSpeed;

            if (isChanged) {
                VirtualRoot.RaiseEvent(new GpuStateChangedEvent(Guid.Empty, gpu));
            }
        }

        public GpuType GpuType {
            get {
                return GpuType.AMD;
            }
        }

        public string DriverVersion {
            get {
                return _driverVersion.ToString();
            }
        }

        public bool IsLowDriverVersion {
            get {
                return this._driverVersion < NTMinerContext.Instance.MinAmdDriverVersion;
            }
        }

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
