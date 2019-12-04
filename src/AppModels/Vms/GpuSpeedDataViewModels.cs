using NTMiner.MinerClient;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class GpuSpeedDataViewModels : ViewModelBase {
        private readonly List<GpuSpeedDataViewModel> _gpuSpeeds = new List<GpuSpeedDataViewModel>();
        private string _mainCoinCode;
        private string _dualCoinCode;
        private string _mainCoinSpeedText;
        private string _dualCoinSpeedText;
        private string _powerUsageWText;
        private bool _isRejectOneGpuShare;
        private bool _isFoundOneGpuShare;
        private bool _isGotOneIncorrectGpuShare;
        private int _cpuPerformance;
        private int _cpuTemperature;
        private string _maxTempText;
        private SolidColorBrush _tempForeground;
        private int _maxTemp;

        public GpuSpeedDataViewModels() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public GpuSpeedDataViewModels(
            string mainCoinCode,
            string dualCoinCode,
            string mainCoinTotalSpeedText,
            string dualCoinTotalSpeedText,
            string totalPowerText,
            bool isRejectOneGpuShare,
            bool isFoundOneGpuShare,
            bool isGotOneIncorrectGpuShare,
            int cpuPerformance,
            int cpuTemperature,
            int maxTemp,
            GpuSpeedData[] datas) {
            this._mainCoinCode = mainCoinCode;
            this._dualCoinCode = dualCoinCode;
            this._mainCoinSpeedText = mainCoinTotalSpeedText;
            this._dualCoinSpeedText = dualCoinTotalSpeedText;
            this._powerUsageWText = totalPowerText;
            this._isRejectOneGpuShare = isRejectOneGpuShare;
            this._isFoundOneGpuShare = isFoundOneGpuShare;
            this._isGotOneIncorrectGpuShare = isGotOneIncorrectGpuShare;
            this._cpuPerformance = cpuPerformance;
            this._cpuTemperature = cpuTemperature;
            this._maxTemp = maxTemp;
            this._maxTempText = maxTemp.ToString("f0") + "℃";
            if (datas != null && datas.Length != 0) {
                foreach (var data in datas) {
                    _gpuSpeeds.Add(new GpuSpeedDataViewModel(data));
                }
            }
        }

        public int MaxTemp {
            get => _maxTemp;
            set {
                _maxTemp = value;
                OnPropertyChanged(nameof(MaxTemp));
            }
        }

        public SolidColorBrush TempForeground {
            get => _tempForeground;
            set {
                _tempForeground = value;
                OnPropertyChanged(nameof(TempForeground));
            }
        }

        public string MaxTempText {
            get => _maxTempText;
            set {
                _maxTempText = value;
                OnPropertyChanged(nameof(MaxTempText));
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

        public bool IsRejectOneGpuShare {
            get => _isRejectOneGpuShare;
            set {
                if (value != _isRejectOneGpuShare) {
                    _isRejectOneGpuShare = value;
                    OnPropertyChanged(nameof(IsRejectOneGpuShare));
                }
            }
        }

        public bool IsFoundOneGpuShare {
            get => _isFoundOneGpuShare;
            set {
                if (_isFoundOneGpuShare != value) {
                    _isFoundOneGpuShare = value;
                    OnPropertyChanged(nameof(IsFoundOneGpuShare));
                }
            }
        }

        public bool IsGotOneIncorrectGpuShare {
            get => _isGotOneIncorrectGpuShare;
            set {
                if (_isGotOneIncorrectGpuShare != value) {
                    _isGotOneIncorrectGpuShare = value;
                    OnPropertyChanged(nameof(IsGotOneIncorrectGpuShare));
                }
            }
        }

        public int CpuPerformance {
            get => _cpuPerformance;
            set {
                _cpuPerformance = value;
                OnPropertyChanged(nameof(CpuPerformance));
            }
        }

        public int CpuTemperature {
            get => _cpuTemperature;
            set {
                _cpuTemperature = value;
                OnPropertyChanged(nameof(CpuTemperature));
            }
        }

        public List<GpuSpeedDataViewModel> List {
            get {
                return _gpuSpeeds;
            }
        }

        public IEnumerable<GpuSpeedDataViewModel> Items {
            get {
                return _gpuSpeeds;
            }
        }
    }
}