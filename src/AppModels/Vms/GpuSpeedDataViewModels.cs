using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class GpuSpeedDataViewModels : ViewModelBase {
        private readonly List<GpuSpeedDataViewModel> _gpuSpeeds = new List<GpuSpeedDataViewModel>();
        private string _mainCoinCode;
        private string _dualCoinCode;
        private string _mainCoinSpeedText;
        private string _dualCoinSpeedText;
        private string _powerUsageWText;
        private string _temperatureSumText;

        public GpuSpeedDataViewModels(
            string mainCoinCode,
            string dualCoinCode,
            string mainCoinTotalSpeedText,
            string dualCoinTotalSpeedText,
            string totalPowerText,
            string temperatureSumText,
            GpuSpeedData[] datas) {
            this._mainCoinCode = mainCoinCode;
            this._dualCoinCode = dualCoinCode;
            this._mainCoinSpeedText = mainCoinTotalSpeedText;
            this._dualCoinSpeedText = dualCoinTotalSpeedText;
            this._powerUsageWText = totalPowerText;
            this._temperatureSumText = temperatureSumText;
            if (datas != null && datas.Length != 0) {
                foreach (var data in datas) {
                    _gpuSpeeds.Add(new GpuSpeedDataViewModel(data));
                }
            }
        }

        public string MainCoinCode {
            get => _mainCoinCode;
            set {
                _mainCoinCode = value;
                OnPropertyChanged(nameof(MainCoinCode));
            }
        }
        public string DualCoinCode {
            get => _dualCoinCode;
            set {
                _dualCoinCode = value;
                OnPropertyChanged(nameof(DualCoinCode));
            }
        }
        public string MainCoinSpeedText {
            get => _mainCoinSpeedText;
            set {
                _mainCoinSpeedText = value;
                OnPropertyChanged(nameof(MainCoinSpeedText));
            }
        }
        public string DualCoinSpeedText {
            get => _dualCoinSpeedText;
            set {
                _dualCoinSpeedText = value;
                OnPropertyChanged(nameof(DualCoinSpeedText));
            }
        }
        public string PowerUsageWText {
            get => _powerUsageWText;
            set {
                _powerUsageWText = value;
                OnPropertyChanged(nameof(PowerUsageWText));
            }
        }

        public string TemperatureSumText {
            get => _temperatureSumText;
            set {
                _temperatureSumText = value;
                OnPropertyChanged(nameof(TemperatureSumText));
            }
        }

        public List<GpuSpeedDataViewModel> List {
            get {
                return _gpuSpeeds;
            }
        }
    }
}