using NTMiner.Core.Gpus;
using System.Linq;

namespace NTMiner.Vms {
    public class GpuViewModel : ViewModelBase, IGpu {
        private readonly IGpu _gpu;

        public GpuViewModel(IGpu gpu) {
            _gpu = gpu;
        }

        public IOverClock OverClock {
            get {
                return _gpu.OverClock;
            }
        }

        public int Index {
            get => _gpu.Index;
        }

        public string IndexText {
            get {
                if (Index == NTMinerRoot.GpuAllId) {
                    return "All";
                }
                return $"GPU{Index}";
            }
        }

        public string Name {
            get => _gpu.Name;
        }

        public uint Temperature {
            get => _gpu.Temperature;
            set {
                if (_gpu.Temperature != value) {
                    _gpu.Temperature = value;
                    OnPropertyChanged(nameof(Temperature));
                    OnPropertyChanged(nameof(TemperatureText));
                }
            }
        }

        public string TemperatureText {
            get {
                if (NTMinerRoot.Current.GpuSet == EmptyGpuSet.Instance) {
                    return "0℃";
                }
                if (this.Index == NTMinerRoot.GpuAllId && NTMinerRoot.Current.GpuSet.Count != 0) {
                    uint min = uint.MaxValue, max = uint.MinValue;
                    foreach (var item in GpuViewModels.Current) {
                        if (item.Index == NTMinerRoot.GpuAllId) {
                            continue;
                        }
                        if (item.Temperature > max) {
                            max = item.Temperature;
                        }
                        if (item.Temperature < min) {
                            min = item.Temperature;
                        }
                    }
                    return $"{min} - {max}℃";
                }
                return this.Temperature.ToString() + "℃";
            }
        }

        public uint FanSpeed {
            get => _gpu.FanSpeed;
            set {
                if (_gpu.FanSpeed != value) {
                    _gpu.FanSpeed = value;
                    OnPropertyChanged(nameof(FanSpeed));
                    OnPropertyChanged(nameof(FanSpeedText));
                }
            }
        }

        public string FanSpeedText {
            get {
                if (NTMinerRoot.Current.GpuSet == EmptyGpuSet.Instance) {
                    return "0%";
                }
                if (this.Index == NTMinerRoot.GpuAllId && NTMinerRoot.Current.GpuSet.Count != 0) {
                    uint min = uint.MaxValue, max = uint.MinValue;
                    foreach (var item in GpuViewModels.Current) {
                        if (item.Index == NTMinerRoot.GpuAllId) {
                            continue;
                        }
                        if (item.FanSpeed > max) {
                            max = item.FanSpeed;
                        }
                        if (item.FanSpeed < min) {
                            min = item.FanSpeed;
                        }
                    }
                    return $"{min} - {max}%";
                }
                return this.FanSpeed.ToString() + "%";
            }
        }

        public uint PowerUsage {
            get => _gpu.PowerUsage;
            set {
                if (_gpu.PowerUsage != value) {
                    _gpu.PowerUsage = value;
                    OnPropertyChanged(nameof(PowerUsage));
                    OnPropertyChanged(nameof(PowerUsageW));
                    OnPropertyChanged(nameof(PowerUsageWText));
                }
            }
        }

        public double PowerUsageW {
            get {
                return this.PowerUsage;
            }
        }

        public string PowerUsageWText {
            get {
                if (NTMinerRoot.Current.GpuSet == EmptyGpuSet.Instance) {
                    return "0W";
                }
                if (this.Index == NTMinerRoot.GpuAllId && NTMinerRoot.Current.GpuSet.Count != 0) {
                    return $"{(GpuViewModels.Current.Sum(a => a.PowerUsage)).ToString("f0")}W";
                }
                return PowerUsageW.ToString("f0") + "W";
            }
        }

        public int CoreClockDelta {
            get { return _gpu.CoreClockDelta; }
            set {
                _gpu.CoreClockDelta = value;
                OnPropertyChanged(nameof(CoreClockDelta));
                OnPropertyChanged(nameof(CoreClockDeltaMText));
            }
        }

        public string CoreClockDeltaMText {
            get {
                if (NTMinerRoot.Current.GpuSet == EmptyGpuSet.Instance) {
                    return "0M";
                }
                if (this.Index == NTMinerRoot.GpuAllId && NTMinerRoot.Current.GpuSet.Count != 0) {
                    int min = int.MaxValue, max = int.MinValue;
                    foreach (var item in GpuViewModels.Current) {
                        if (item.Index == NTMinerRoot.GpuAllId) {
                            continue;
                        }
                        if (item.GpuClockDeltaVm.CoreClockDeltaMax > max) {
                            max = item.GpuClockDeltaVm.CoreClockDeltaMax;
                        }
                        if (item.GpuClockDeltaVm.CoreClockDeltaMin < min) {
                            min = item.GpuClockDeltaVm.CoreClockDeltaMin;
                        }
                    }
                    return $"{min/1000} - {max/1000}M";
                }
                return (this.CoreClockDelta / 1000).ToString();
            }
        }

        public int MemoryClockDelta {
            get { return _gpu.MemoryClockDelta; }
            set {
                _gpu.MemoryClockDelta = value;
                OnPropertyChanged(nameof(MemoryClockDelta));
                OnPropertyChanged(nameof(MemoryClockDeltaMText));
            }
        }

        public string MemoryClockDeltaMText {
            get {
                if (NTMinerRoot.Current.GpuSet == EmptyGpuSet.Instance) {
                    return "0M";
                }
                if (this.Index == NTMinerRoot.GpuAllId && NTMinerRoot.Current.GpuSet.Count != 0) {
                    int min = int.MaxValue, max = int.MinValue;
                    foreach (var item in GpuViewModels.Current) {
                        if (item.Index == NTMinerRoot.GpuAllId) {
                            continue;
                        }
                        if (item.GpuClockDeltaVm.MemoryClockDeltaMax > max) {
                            max = item.GpuClockDeltaVm.MemoryClockDeltaMax;
                        }
                        if (item.GpuClockDeltaVm.MemoryClockDeltaMin < min) {
                            min = item.GpuClockDeltaVm.MemoryClockDeltaMin;
                        }
                    }
                    return $"{min / 1000} - {max / 1000}M";
                }
                return (this.MemoryClockDelta / 1000).ToString();
            }
        }

        private GpuClockDeltaViewModel _gpuClockDeltaVm;
        public GpuClockDeltaViewModel GpuClockDeltaVm {
            get {
                if (_gpuClockDeltaVm == null) {
                    IGpuClockDelta delta;
                    if (NTMinerRoot.Current.GpuSet.GpuClockDeltaSet.TryGetValue(this.Index, out delta)) {
                        _gpuClockDeltaVm = new GpuClockDeltaViewModel(delta);
                    }
                    else {
                        _gpuClockDeltaVm = GpuClockDeltaViewModel.Empty;
                    }
                }
                return _gpuClockDeltaVm;
            }
        }
    }
}
