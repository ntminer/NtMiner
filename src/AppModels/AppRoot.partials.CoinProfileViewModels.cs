using NTMiner.Vms;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static partial class AppRoot {
        public class CoinProfileViewModels : ViewModelBase {
            private readonly object _locker = new object();
            public static CoinProfileViewModels Instance { get; private set; } = new CoinProfileViewModels();

            private readonly Dictionary<Guid, CoinKernelProfileViewModel> _coinKernelProfileDicById = new Dictionary<Guid, CoinKernelProfileViewModel>();
            private readonly Dictionary<Guid, CoinProfileViewModel> _coinProfileDicById = new Dictionary<Guid, CoinProfileViewModel>();

            private CoinProfileViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                BuildEventPath<CoinKernelProfilePropertyChangedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_coinKernelProfileDicById.ContainsKey(message.CoinKernelId)) {
                            _coinKernelProfileDicById[message.CoinKernelId].OnPropertyChanged(message.PropertyName);
                        }
                    });
                BuildEventPath<CoinProfilePropertyChangedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_coinProfileDicById.ContainsKey(message.CoinId)) {
                            _coinProfileDicById[message.CoinId].OnPropertyChanged(message.PropertyName);
                        }
                    });
                VirtualRoot.BuildEventPath<LocalContextReInitedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        _coinKernelProfileDicById.Clear();
                        _coinProfileDicById.Clear();
                    });
            }

            public CoinProfileViewModel GetOrCreateCoinProfile(Guid coinId) {
                if (!_coinProfileDicById.TryGetValue(coinId, out CoinProfileViewModel coinProfile)) {
                    lock (_locker) {
                        if (!_coinProfileDicById.TryGetValue(coinId, out coinProfile)) {
                            coinProfile = new CoinProfileViewModel(NTMinerContext.Instance.MinerProfile.GetCoinProfile(coinId));
                            _coinProfileDicById.Add(coinId, coinProfile);
                        }
                    }
                }
                return coinProfile;
            }

            public CoinKernelProfileViewModel GetOrCreateCoinKernelProfileVm(Guid coinKernelId) {
                if (!_coinKernelProfileDicById.TryGetValue(coinKernelId, out CoinKernelProfileViewModel coinKernelProfileVm)) {
                    lock (_locker) {
                        if (!_coinKernelProfileDicById.TryGetValue(coinKernelId, out coinKernelProfileVm)) {
                            coinKernelProfileVm = new CoinKernelProfileViewModel(NTMinerContext.Instance.MinerProfile.GetCoinKernelProfile(coinKernelId));
                            _coinKernelProfileDicById.Add(coinKernelId, coinKernelProfileVm);
                        }
                    }
                }

                return coinKernelProfileVm;
            }
        }
    }
}
