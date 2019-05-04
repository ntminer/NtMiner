using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.Vms;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner {
    public partial class AppContext {
        public class GpuViewModels : ViewModelBase, IEnumerable<GpuViewModel> {
            public static readonly GpuViewModels Instance = new GpuViewModels();

            private Dictionary<int, GpuViewModel> _gpuVms = new Dictionary<int, GpuViewModel>();

            private string _fanSpeedMinText = "0 %";
            private string _fanSpeedMaxText = "0 %";
            private string _temperatureMinText = "0 ℃";
            private string _temperatureMaxText = "0 ℃";
            private readonly GpuViewModel _totalGpuVm;
            private GpuViewModels() {
                if (Design.IsInDesignMode) {
                    return;
                }
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    _gpuVms.Add(gpu.Index, new GpuViewModel(gpu));
                }
                if (_gpuVms.ContainsKey(NTMinerRoot.GpuAllId)) {
                    _totalGpuVm = _gpuVms[NTMinerRoot.GpuAllId];
                }
                On<Per5SecondEvent>("周期刷新显卡状态", LogEnum.None,
                    action: message => {
                        NTMinerRoot.Instance.GpuSet.LoadGpuState();
                    });
                On<GpuStateChangedEvent>("显卡状态变更后刷新VM内存", LogEnum.None,
                    action: message => {
                        if (_gpuVms.ContainsKey(message.Source.Index)) {
                            GpuViewModel vm = _gpuVms[message.Source.Index];
                            vm.Temperature = message.Source.Temperature;
                            vm.FanSpeed = message.Source.FanSpeed;
                            vm.PowerUsage = message.Source.PowerUsage;
                            vm.CoreClockDelta = message.Source.CoreClockDelta;
                            vm.MemoryClockDelta = message.Source.MemoryClockDelta;
                            vm.CoreClockDeltaMin = message.Source.CoreClockDeltaMin;
                            vm.CoreClockDeltaMax = message.Source.CoreClockDeltaMax;
                            vm.MemoryClockDeltaMin = message.Source.MemoryClockDeltaMin;
                            vm.MemoryClockDeltaMax = message.Source.MemoryClockDeltaMax;
                            vm.Cool = message.Source.Cool;
                            vm.CoolMin = message.Source.CoolMin;
                            vm.CoolMax = message.Source.CoolMax;
                            vm.PowerCapacity = message.Source.PowerCapacity;
                            vm.PowerMin = message.Source.PowerMin;
                            vm.PowerMax = message.Source.PowerMax;
                            vm.TempLimit = message.Source.TempLimit;
                            vm.TempLimitDefault = message.Source.TempLimitDefault;
                            vm.TempLimitMax = message.Source.TempLimitMax;
                            vm.TempLimitMin = message.Source.TempLimitMin;
                            if (_totalGpuVm != null) {
                                _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.TemperatureText));
                                _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.FanSpeedText));
                                _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.PowerUsageWText));
                                _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.CoreClockDeltaMText));
                                _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.MemoryClockDeltaMText));
                                _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.CoreClockDeltaMinMaxMText));
                                _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.MemoryClockDeltaMinMaxMText));
                                _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.CoolMinMaxText));
                                _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.PowerMinMaxText));
                                _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.TempLimitMinMaxText));
                            }
                            UpdateMinMax();
                        }
                    });
            }

            private void UpdateMinMax() {
                uint minFan = uint.MaxValue, maxFan = uint.MinValue;
                foreach (var item in _gpuVms.Values) {
                    if (item.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (item.FanSpeed > maxFan) {
                        maxFan = item.FanSpeed;
                    }
                    if (item.FanSpeed < minFan) {
                        minFan = item.FanSpeed;
                    }
                }
                this.FanSpeedMaxText = maxFan + " %";
                this.FanSpeedMinText = minFan + " %";
                int minTemp = int.MaxValue, maxTemp = int.MinValue;
                foreach (var item in _gpuVms.Values) {
                    if (item.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (item.Temperature > maxTemp) {
                        maxTemp = item.Temperature;
                    }
                    if (item.Temperature < minTemp) {
                        minTemp = item.Temperature;
                    }
                }
                this.TemperatureMaxText = maxTemp + " ℃";
                this.TemperatureMinText = minTemp + " ℃";
            }

            public int Count {
                get {
                    if (_totalGpuVm != null) {
                        return _gpuVms.Count - 1;
                    }
                    return _gpuVms.Count;
                }
            }

            public string TemperatureMinText {
                get => _temperatureMinText;
                set {
                    _temperatureMinText = value;
                    OnPropertyChanged(nameof(TemperatureMinText));
                }
            }

            public string TemperatureMaxText {
                get => _temperatureMaxText;
                set {
                    _temperatureMaxText = value;
                    OnPropertyChanged(nameof(TemperatureMaxText));
                }
            }

            public string FanSpeedMinText {
                get => _fanSpeedMinText;
                set {
                    _fanSpeedMinText = value;
                    OnPropertyChanged(nameof(FanSpeedMinText));
                }
            }

            public string FanSpeedMaxText {
                get { return _fanSpeedMaxText; }
                set {
                    _fanSpeedMaxText = value;
                    OnPropertyChanged(nameof(FanSpeedMaxText));
                }
            }

            public bool TryGetGpuVm(int index, out GpuViewModel gpuVm) {
                return _gpuVms.TryGetValue(index, out gpuVm);
            }

            public IEnumerator<GpuViewModel> GetEnumerator() {
                return _gpuVms.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return _gpuVms.Values.GetEnumerator();
            }
        }
    }
}
