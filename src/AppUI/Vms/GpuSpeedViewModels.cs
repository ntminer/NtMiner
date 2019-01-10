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
        private double _incomeDualCoinPerDay;

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
            Global.Access<GpuSpeedChangedEvent>(
                Guid.Parse("acb2e5fd-a3ed-4ed6-b8c7-583eafd5e579"),
                "显卡算力变更后刷新VM内存",
                LogEnum.None,
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
                        IncomeDualCoinPerDay = 0;
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
                            IncomeDualCoinPerDay = 0;
                        }
                        else {
                            if (message.IsDualSpeed) {
                                if (mineContext is IDualMineContext dualMineContext) {
                                    IncomeDualCoinPerDay = _totalSpeedVm.DualCoinSpeed.Value * NTMinerRoot.Current.CalcConfigSet.GetIncomePerHashPerDay(dualMineContext.DualCoin);
                                }
                            }
                            else {
                                IncomeMainCoinPerDay = _totalSpeedVm.MainCoinSpeed.Value *NTMinerRoot.Current.CalcConfigSet.GetIncomePerHashPerDay(mineContext.MainCoin);
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
