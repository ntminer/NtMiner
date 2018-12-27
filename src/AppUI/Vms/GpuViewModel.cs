using NTMiner.Core.Gpus;
using System.Linq;

namespace NTMiner.Vms {
    public class GpuViewModel : ViewModelBase, IGpu {
        private readonly IGpu _gpu;
        public GpuViewModel(IGpu gpu) {
            _gpu = gpu;
        }

        public int Index {
            get => _gpu.Index;
        }

        public string IndexText {
            get {
                if (Index == NTMinerRoot.Current.GpuAllId) {
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
                _gpu.Temperature = value;
                OnPropertyChanged(nameof(Temperature));
                OnPropertyChanged(nameof(TemperatureText));
            }
        }

        public string TemperatureText {
            get {
                if (NTMinerRoot.Current.GpuSet == EmptyGpuSet.Instance) {
                    return "0℃";
                }
                if (this.Index == NTMinerRoot.Current.GpuAllId) {
                    uint min = uint.MaxValue, max = uint.MinValue;
                    foreach (var item in GpuViewModels.Current) {
                        if (item.Index == NTMinerRoot.Current.GpuAllId) {
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
                _gpu.FanSpeed = value;
                OnPropertyChanged(nameof(FanSpeed));
                OnPropertyChanged(nameof(FanSpeedText));
            }
        }

        public string FanSpeedText {
            get {
                if (NTMinerRoot.Current.GpuSet == EmptyGpuSet.Instance) {
                    return "0%";
                }
                if (this.Index == NTMinerRoot.Current.GpuAllId) {
                    uint min = uint.MaxValue, max = uint.MinValue;
                    foreach (var item in GpuViewModels.Current) {
                        if (item.Index == NTMinerRoot.Current.GpuAllId) {
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
                _gpu.PowerUsage = value;
                OnPropertyChanged(nameof(PowerUsage));
                OnPropertyChanged(nameof(PowerUsageW));
                OnPropertyChanged(nameof(PowerUsageWText));
            }
        }

        public double PowerUsageW {
            get {
                return this.PowerUsage / 1000.0;
            }
        }

        public string PowerUsageWText {
            get {
                if (NTMinerRoot.Current.GpuSet == EmptyGpuSet.Instance) {
                    return "0W";
                }
                if (this.Index == NTMinerRoot.Current.GpuAllId) {
                    return $"{(GpuViewModels.Current.Sum(a => a.PowerUsage) / 1000.0).ToString("f0")}W";
                }
                return PowerUsageW.ToString("f0") + "W";
            }
        }
    }
}
