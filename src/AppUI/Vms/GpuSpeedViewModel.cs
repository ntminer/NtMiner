using NTMiner.Core.Gpus;
using NTMiner.Views.Ucs;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class GpuSpeedViewModel : ViewModelBase {
        private SpeedViewModel _mainCoinSpeed;
        private SpeedViewModel _dualCoinSpeed;

        private readonly IGpuSpeed _gpuSpeed;

        public ICommand OpenChart { get; private set; }

        public GpuSpeedViewModel(IGpuSpeed gpuSpeed) {
            _gpuSpeed = gpuSpeed;
            this._mainCoinSpeed = new SpeedViewModel(gpuSpeed.MainCoinSpeed);
            this._dualCoinSpeed = new SpeedViewModel(gpuSpeed.DualCoinSpeed);
            this.OpenChart = new DelegateCommand(() => {
                SpeedCharts.ShowWindow(this);
            });
        }

        private GpuViewModel _gpuVm = null;
        public GpuViewModel GpuVm {
            get {
                if (_gpuVm == null) {
                    _gpuVm = GpuViewModels.Current.FirstOrDefault(a => a.Index == _gpuSpeed.Gpu.Index);
                }
                return _gpuVm;
            }
        }

        public SpeedViewModel MainCoinSpeed {
            get => _mainCoinSpeed;
            set {
                _mainCoinSpeed = value;
                OnPropertyChanged(nameof(MainCoinSpeed));
            }
        }

        public SpeedViewModel DualCoinSpeed {
            get => _dualCoinSpeed;
            set {
                _dualCoinSpeed = value;
                OnPropertyChanged(nameof(DualCoinSpeed));
            }
        }
    }
}
