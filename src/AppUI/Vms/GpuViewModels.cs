using NTMiner.Core;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class GpuViewModels : ViewModelBase, IEnumerable<GpuViewModel> {
        public static readonly GpuViewModels Current = new GpuViewModels();
        private Dictionary<int, GpuViewModel> _gpuVms = new Dictionary<int, GpuViewModel>();

        private readonly GpuViewModel _totalGpuVm;
        private GpuViewModels() {
            if (Design.IsInDesignMode) {
                return;
            }
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                _gpuVms.Add(gpu.Index, new GpuViewModel(gpu));
            }
            if (_gpuVms.ContainsKey(NTMinerRoot.GpuAllId)) {
                _totalGpuVm = _gpuVms[NTMinerRoot.GpuAllId];
            }
            VirtualRoot.On<GpuStateChangedEvent>("显卡状态变更后刷新VM内存", LogEnum.None,
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
                        vm.Power = message.Source.Power;
                        vm.PowerMin = message.Source.PowerMin;
                        vm.PowerMax = message.Source.PowerMax;
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
                        }
                    }
                });
        }

        public int Count {
            get {
                if (_totalGpuVm != null) {
                    return _gpuVms.Count - 1;
                }
                return _gpuVms.Count;
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
