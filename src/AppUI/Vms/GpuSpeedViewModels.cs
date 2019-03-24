using NTMiner.Core;
using NTMiner.Core.Gpus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class GpuSpeedViewModels : ViewModelBase {
        public static readonly GpuSpeedViewModels Current = new GpuSpeedViewModels();
        private readonly List<GpuSpeedViewModel> _list = new List<GpuSpeedViewModel>();
        private readonly GpuSpeedViewModel _totalSpeedVm;

        private Guid _mainCoinId;
        private double _incomeMainCoinPerDay;
        private double _incomeMainCoinUsdPerDay;
        private double _incomeMainCoinCnyPerDay;
        private double _incomeDualCoinPerDay;
        private double _incomeDualCoinUsdPerDay;
        private double _incomeDualCoinCnyPerDay;

        private GpuSpeedViewModels() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.GpuAllVm = GpuViewModels.Current.FirstOrDefault(a => a.Index == NTMinerRoot.GpuAllId);
            IGpusSpeed gpuSpeeds = NTMinerRoot.Current.GpusSpeed;
            foreach (var item in gpuSpeeds) {
                this._list.Add(new GpuSpeedViewModel(item));
            }
            _totalSpeedVm = this._list.FirstOrDefault(a => a.GpuVm.Index == NTMinerRoot.GpuAllId);
            VirtualRoot.On<GpuSpeedChangedEvent>("显卡算力变更后刷新VM内存", LogEnum.DevConsole,
                action: (message) => {
                    Guid mainCoinId = NTMinerRoot.Current.MinerProfile.CoinId;
                    if (_mainCoinId != mainCoinId) {
                        _mainCoinId = mainCoinId;
                        DateTime now = DateTime.Now;
                        foreach (var item in _list) {
                            item.MainCoinSpeed.Value = 0;
                            item.MainCoinSpeed.SpeedOn = now;
                            item.DualCoinSpeed.Value = 0;
                            item.DualCoinSpeed.SpeedOn = now;
                        }
                        IncomeMainCoinPerDay = 0;
                        IncomeMainCoinUsdPerDay = 0;
                        IncomeMainCoinCnyPerDay = 0;
                        IncomeDualCoinPerDay = 0;
                        IncomeDualCoinUsdPerDay = 0;
                        IncomeDualCoinCnyPerDay = 0;
                    }
                    int index = message.Source.Gpu.Index;
                    GpuSpeedViewModel gpuSpeedVm = _list.FirstOrDefault(a => a.GpuVm.Index == index);
                    if (gpuSpeedVm != null) {
                        if (message.IsDualSpeed) {
                            gpuSpeedVm.DualCoinSpeed.Update(message.Source.DualCoinSpeed);
                        }
                        else {
                            gpuSpeedVm.MainCoinSpeed.Update(message.Source.MainCoinSpeed);
                        }
                    }
                    if (index == _totalSpeedVm.GpuVm.Index) {
                        IMineContext mineContext = NTMinerRoot.Current.CurrentMineContext;
                        if (mineContext == null) {
                            IncomeMainCoinPerDay = 0;
                            IncomeMainCoinUsdPerDay = 0;
                            IncomeMainCoinCnyPerDay = 0;
                            IncomeDualCoinPerDay = 0;
                            IncomeDualCoinUsdPerDay = 0;
                            IncomeDualCoinCnyPerDay = 0;
                        }
                        else {
                            if (message.IsDualSpeed) {
                                if (mineContext is IDualMineContext dualMineContext) {
                                    IncomePerDay incomePerDay = NTMinerRoot.Current.CalcConfigSet.GetIncomePerHashPerDay(dualMineContext.DualCoin.Code);
                                    IncomeDualCoinPerDay = _totalSpeedVm.DualCoinSpeed.Value * incomePerDay.IncomeCoin;
                                    IncomeDualCoinUsdPerDay = _totalSpeedVm.DualCoinSpeed.Value * incomePerDay.IncomeUsd;
                                    IncomeDualCoinCnyPerDay = _totalSpeedVm.DualCoinSpeed.Value * incomePerDay.IncomeCny;
                                }
                            }
                            else {
                                IncomePerDay incomePerDay = NTMinerRoot.Current.CalcConfigSet.GetIncomePerHashPerDay(mineContext.MainCoin.Code);
                                IncomeMainCoinPerDay = _totalSpeedVm.MainCoinSpeed.Value * incomePerDay.IncomeCoin;
                                IncomeMainCoinUsdPerDay = _totalSpeedVm.MainCoinSpeed.Value * incomePerDay.IncomeUsd;
                                IncomeMainCoinCnyPerDay = _totalSpeedVm.MainCoinSpeed.Value * incomePerDay.IncomeCny;
                            }
                        }
                    }
                });
        }

        public GpuViewModel GpuAllVm {
            get; set;
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }

        public int Count {
            get {
                if (_totalSpeedVm != null) {
                    return _list.Count - 1;
                }
                return _list.Count;
            }
        }

        public double IncomeMainCoinPerDay {
            get => _incomeMainCoinPerDay;
            set {
                _incomeMainCoinPerDay = value;
                OnPropertyChanged(nameof(IncomeMainCoinPerDay));
                OnPropertyChanged(nameof(IncomeMainCoinPerDayText));
            }
        }

        public string IncomeMainCoinPerDayText {
            get {
                return IncomeMainCoinPerDay.ToString("f7");
            }
        }

        public double IncomeMainCoinUsdPerDay {
            get { return _incomeMainCoinUsdPerDay; }
            set {
                _incomeMainCoinUsdPerDay = value;
                OnPropertyChanged(nameof(IncomeMainCoinUsdPerDay));
                OnPropertyChanged(nameof(IncomeMainCoinUsdPerDayText));
            }
        }

        public string IncomeMainCoinUsdPerDayText {
            get {
                return IncomeMainCoinUsdPerDay.ToString("f2");
            }
        }

        public double IncomeMainCoinCnyPerDay {
            get { return _incomeMainCoinCnyPerDay; }
            set {
                _incomeMainCoinCnyPerDay = value;
                OnPropertyChanged(nameof(IncomeMainCoinCnyPerDay));
                OnPropertyChanged(nameof(IncomeMainCoinCnyPerDayText));
            }
        }

        public string IncomeMainCoinCnyPerDayText {
            get {
                return IncomeMainCoinCnyPerDay.ToString("f2");
            }
        }

        public double IncomeDualCoinPerDay {
            get => _incomeDualCoinPerDay;
            set {
                _incomeDualCoinPerDay = value;
                OnPropertyChanged(nameof(IncomeDualCoinPerDay));
                OnPropertyChanged(nameof(IncomeDualCoinPerDayText));
            }
        }

        public string IncomeDualCoinPerDayText {
            get {
                return IncomeDualCoinPerDay.ToString("f7");
            }
        }

        public double IncomeDualCoinUsdPerDay {
            get { return _incomeDualCoinUsdPerDay; }
            set {
                _incomeDualCoinUsdPerDay = value;
                OnPropertyChanged(nameof(IncomeDualCoinUsdPerDay));
                OnPropertyChanged(nameof(IncomeDualCoinUsdPerDayText));
            }
        }

        public string IncomeDualCoinUsdPerDayText {
            get {
                return IncomeDualCoinUsdPerDay.ToString("f2");
            }
        }

        public double IncomeDualCoinCnyPerDay {
            get { return _incomeDualCoinCnyPerDay; }
            set {
                _incomeDualCoinCnyPerDay = value;
                OnPropertyChanged(nameof(IncomeDualCoinCnyPerDay));
                OnPropertyChanged(nameof(IncomeDualCoinCnyPerDayText));
            }
        }

        public string IncomeDualCoinCnyPerDayText {
            get {
                return IncomeDualCoinCnyPerDay.ToString("f2");
            }
        }

        public GpuSpeedViewModel TotalSpeedVm {
            get { return _totalSpeedVm; }
        }

        public List<GpuSpeedViewModel> GpuSpeedVms {
            get {
                return _list.Where(a => a.GpuVm.Index != NTMinerRoot.GpuAllId).ToList();
            }
        }

        public List<GpuSpeedViewModel> AllGpuSpeedVms {
            get {
                return _list;
            }
        }
    }
}
