using NTMiner.Core;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
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
                    NTMinerRoot.Current.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(CoinKernelId), value);
                    OnPropertyChanged(nameof(CoinKernelId));
                }
            }
        }

        public bool IsDualCoinEnabled {
            get {
                if (NTMinerRoot.Current.CoinKernelSet.TryGetCoinKernel(this.CoinKernelId, out ICoinKernel coinKernelVm) && !coinKernelVm.IsSupportDualMine()) {
                    return false;
                }
                return _inner.IsDualCoinEnabled;
            }
            set {
                if (_inner.IsDualCoinEnabled != value) {
                    NTMinerRoot.Current.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(IsDualCoinEnabled), value);
                    OnPropertyChanged(nameof(IsDualCoinEnabled));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }
        public Guid DualCoinId {
            get => _inner.DualCoinId;
            set {
                if (_inner.DualCoinId != value) {
                    NTMinerRoot.Current.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(DualCoinId), value);
                    OnPropertyChanged(nameof(DualCoinId));
                }
            }
        }

        private double _dualCoinWeight;
        public double DualCoinWeight {
            get => _dualCoinWeight;
            set {
                if (Math.Abs(_dualCoinWeight - value) > 0.01) {
                    _dualCoinWeight = value;
                    OnPropertyChanged(nameof(DualCoinWeight));
                }
            }
        }

        public bool IsAutoDualWeight {
            get => _inner.IsAutoDualWeight;
            set {
                if (_inner.IsAutoDualWeight != value) {
                    NTMinerRoot.Current.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(IsAutoDualWeight), value);
                    OnPropertyChanged(nameof(IsAutoDualWeight));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }

        public string CustomArgs {
            get => _inner.CustomArgs;
            set {
                if (_inner.CustomArgs != value) {
                    NTMinerRoot.Current.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(CustomArgs), value);
                    OnPropertyChanged(nameof(CustomArgs));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }

        public CoinViewModel SelectedDualCoin {
            get {
                if (!CoinViewModels.Current.TryGetCoinVm(this.DualCoinId, out CoinViewModel coin)) {
                    List<ICoin> dualCoins = this.GetDualCoins();
                    if (dualCoins != null && dualCoins.Count != 0) {
                        coin = CoinViewModels.Current.AllCoins.FirstOrDefault(a => a.Id == dualCoins[0].GetId());
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
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }
    }
}
