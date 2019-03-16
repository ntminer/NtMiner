using NTMiner.Core.Gpus;
using NTMiner.Views.Ucs;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class GpuSpeedViewModel : ViewModelBase {
        private SpeedViewModel _mainCoinSpeed;
        private SpeedViewModel _dualCoinSpeed;

        public ICommand OpenChart { get; private set; }

        public GpuSpeedViewModel(IGpuSpeed gpuSpeed) {
            GpuViewModel gpuVm;
            GpuViewModels.Current.TryGetGpuVm(gpuSpeed.Gpu.Index, out gpuVm);
            _gpuVm = gpuVm;
            this._mainCoinSpeed = new SpeedViewModel(gpuSpeed.MainCoinSpeed);
            this._dualCoinSpeed = new SpeedViewModel(gpuSpeed.DualCoinSpeed);
            this.OpenChart = new DelegateCommand(() => {
                SpeedCharts.ShowWindow(this);
            });
        }

        private GpuViewModel _gpuVm = null;
        public GpuViewModel GpuVm {
            get {
                return _gpuVm;
            }
        }

        public SpeedViewModel MainCoinSpeed {
            get => _mainCoinSpeed;
            set {
                if (_mainCoinSpeed != value) {
                    _mainCoinSpeed = value;
                    OnPropertyChanged(nameof(MainCoinSpeed));
                }
            }
        }

        public SpeedViewModel DualCoinSpeed {
            get => _dualCoinSpeed;
            set {
                if (_dualCoinSpeed != value) {
                    _dualCoinSpeed = value;
                    OnPropertyChanged(nameof(DualCoinSpeed));
                }
            }
        }
    }
}
