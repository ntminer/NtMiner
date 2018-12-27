using NTMiner.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class GpuViewModels : ViewModelBase, IEnumerable<GpuViewModel> {
        public static readonly GpuViewModels Current = new GpuViewModels();
        private Dictionary<int, GpuViewModel> _gpuVms = new Dictionary<int, GpuViewModel>();

        private readonly GpuViewModel _totalGpuVm;
        private GpuViewModels() {
            if (NTMinerRoot.IsInDesignMode) {
                return;
            }
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                _gpuVms.Add(gpu.Index, new GpuViewModel(gpu));
            }
            if (_gpuVms.ContainsKey(NTMinerRoot.Current.GpuAllId)) {
                _totalGpuVm = _gpuVms[NTMinerRoot.Current.GpuAllId];
            }
            Global.Access<GpuStateChangedEvent>(
                Guid.Parse("57a0dc71-08ca-4cde-9e7e-214ed3cfaf04"),
                "显卡状态变更后刷新VM内存",
                LogEnum.None,
                action: message => {
                    if (_gpuVms.ContainsKey(message.Source.Index)) {
                        GpuViewModel vm = _gpuVms[message.Source.Index];
                        vm.Temperature = message.Source.Temperature;
                        vm.FanSpeed = message.Source.FanSpeed;
                        vm.PowerUsage = message.Source.PowerUsage;
                        if (_totalGpuVm != null) {
                            _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.TemperatureText));
                            _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.FanSpeedText));
                            _totalGpuVm.OnPropertyChanged(nameof(_totalGpuVm.PowerUsageWText));
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

        public IEnumerator<GpuViewModel> GetEnumerator() {
            return _gpuVms.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _gpuVms.Values.GetEnumerator();
        }
    }
}
