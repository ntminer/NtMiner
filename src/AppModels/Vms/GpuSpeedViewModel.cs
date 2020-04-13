using NTMiner.Core;
using NTMiner.Core.Gpus;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class GpuSpeedViewModel : ViewModelBase {
        private SpeedViewModel _mainCoinSpeed;
        private SpeedViewModel _dualCoinSpeed;

        public ICommand OpenChart { get; private set; }

        public GpuSpeedViewModel(IGpuSpeed gpuSpeed) {
            GpuViewModel gpuVm;
            AppRoot.GpuVms.TryGetGpuVm(gpuSpeed.Gpu.Index, out gpuVm);
            _gpuVm = gpuVm;
            this._mainCoinSpeed = new SpeedViewModel(gpuSpeed.MainCoinSpeed);
            this._dualCoinSpeed = new SpeedViewModel(gpuSpeed.DualCoinSpeed);
            this.OpenChart = new DelegateCommand(() => {
                VirtualRoot.Execute(new ShowSpeedChartsCommand(this));
            });
        }

        private GpuViewModel _gpuVm = null;
        public GpuViewModel GpuVm {
            get {
                return _gpuVm;
            }
        }

        private Guid _coinId;
        private GpuProfileViewModel _gpuProfileVm;
        public GpuProfileViewModel GpuProfileVm {
            get {
                var coinId = NTMinerContext.Instance.MinerProfile.CoinId;
                if (coinId == Guid.Empty) {
                    return null;
                }
                if (coinId == _coinId) {
                    return _gpuProfileVm;
                }
                _coinId = coinId;
                _gpuProfileVm = AppRoot.GpuProfileViewModels.Instance.List(coinId).FirstOrDefault(a => a.Index == this.GpuVm.Index);
                return _gpuProfileVm;
            }
        }

        public string AverageMainCoinSpeedText {
            get {
                var averageSpeed = NTMinerContext.Instance.GpusSpeed.GetAverageSpeed(GpuVm.Index);
                return averageSpeed.Speed.ToUnitSpeedText();
            }
        }

        public string AverageDualCoinSpeedText {
            get {
                var averageSpeed = NTMinerContext.Instance.GpusSpeed.GetAverageSpeed(GpuVm.Index);
                return averageSpeed.DualSpeed.ToUnitSpeedText();
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
