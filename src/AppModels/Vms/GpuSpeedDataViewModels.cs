using NTMiner.MinerClient;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class GpuSpeedDataViewModels : ViewModelBase, IEnumerable<GpuSpeedDataViewModel> {
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

        public GpuSpeedDataViewModels() {
            if (!Design.IsInDesignMode) {
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

        public IEnumerator<GpuSpeedDataViewModel> GetEnumerator() {
            return _gpuSpeeds.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _gpuSpeeds.GetEnumerator();
        }
    }
}