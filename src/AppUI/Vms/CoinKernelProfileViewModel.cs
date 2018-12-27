using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Linq;

namespace NTMiner.Vms {
    public class CoinKernelProfileViewModel : ViewModelBase, ICoinKernelProfile {
        private readonly ICoinKernelProfile _inner;
        public CoinKernelProfileViewModel(ICoinKernelProfile inner) {
            _inner = inner;
            _dualCoinWeight = inner.DualCoinWeight;
        }

        public Guid CoinKernelId {
            get => _inner.CoinKernelId;
            set {
                if (_inner.CoinKernelId != value) {
                    NTMinerRoot.Current.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(CoinKernelId), value);
                    OnPropertyChanged(nameof(CoinKernelId));
                }
            }
        }

        public bool IsDualCoinEnabled {
            get => _inner.IsDualCoinEnabled;
            set {
                if (_inner.IsDualCoinEnabled != value) {
                    NTMinerRoot.Current.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(IsDualCoinEnabled), value);
                    OnPropertyChanged(nameof(IsDualCoinEnabled));
                    Global.Execute(new RefreshArgsAssemblyCommand());
                }
            }
        }
        public Guid DualCoinId {
            get => _inner.DualCoinId;
            set {
                if (_inner.DualCoinId != value) {
                    NTMinerRoot.Current.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(DualCoinId), value);
                    OnPropertyChanged(nameof(DualCoinId));
                }
            }
        }

        private double _dualCoinWeight;
        private DateTime _dualCoinWeightLastUpdateOn = DateTime.MinValue;
        public double DualCoinWeight {
            get => _dualCoinWeight;
            set {
                if (_dualCoinWeight != value) {
                    _dualCoinWeight = value;
                    OnPropertyChanged(nameof(DualCoinWeight));
                    if (_dualCoinWeightLastUpdateOn.AddSeconds(2) < DateTime.Now) {
                        TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                            NTMinerRoot.Current.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(DualCoinWeight), _dualCoinWeight);
                            Global.Execute(new RefreshArgsAssemblyCommand());
                        });
                    }
                    _dualCoinWeightLastUpdateOn = DateTime.Now;
                }
            }
        }

        public string CustomArgs {
            get => _inner.CustomArgs;
            set {
                if (_inner.CustomArgs != value) {
                    NTMinerRoot.Current.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(CustomArgs), value);
                    OnPropertyChanged(nameof(CustomArgs));
                    Global.Execute(new RefreshArgsAssemblyCommand());
                }
            }
        }

        public CoinViewModel SelectedDualCoin {
            get {
                CoinViewModel coin = CoinViewModels.Current.AllCoins.FirstOrDefault(a => a.Id == DualCoinId);
                if (coin == null) {
                    CoinKernelViewModel coinKernelVm = CoinKernelViewModels.Current.AllCoinKernels.FirstOrDefault(a => a.Id == this.CoinKernelId);
                    if (coinKernelVm != null) {
                        coin = coinKernelVm.DualCoinGroup.DualCoinVms.FirstOrDefault();
                    }
                    if (coin != null) {
                        DualCoinId = coin.Id;
                    }
                }
                return coin;
            }
            set {
                if (value != null && value.Id != Guid.Empty) {
                    DualCoinId = value.Id;
                    OnPropertyChanged(nameof(SelectedDualCoin));
                    Global.Execute(new RefreshArgsAssemblyCommand());
                }
            }
        }
    }
}
