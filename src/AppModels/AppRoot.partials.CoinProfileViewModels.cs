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
                AddEventPath<CoinKernelProfilePropertyChangedEvent>("币种内核设置变更后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_coinKernelProfileDicById.ContainsKey(message.CoinKernelId)) {
                            _coinKernelProfileDicById[message.CoinKernelId].OnPropertyChanged(message.PropertyName);
                        }
                    }, location: this.GetType());
                AddEventPath<CoinProfilePropertyChangedEvent>("币种设置变更后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_coinProfileDicById.ContainsKey(message.CoinId)) {
                            _coinProfileDicById[message.CoinId].OnPropertyChanged(message.PropertyName);
                        }
                    }, location: this.GetType());
                VirtualRoot.AddEventPath<LocalContextReInitedEvent>("LocalContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _coinKernelProfileDicById.Clear();
                        _coinProfileDicById.Clear();
                    }, location: this.GetType());
            }

            public CoinProfileViewModel GetOrCreateCoinProfile(Guid coinId) {
                CoinProfileViewModel coinProfile;
                if (!_coinProfileDicById.TryGetValue(coinId, out coinProfile)) {
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
                CoinKernelProfileViewModel coinKernelProfileVm;
                if (!_coinKernelProfileDicById.TryGetValue(coinKernelId, out coinKernelProfileVm)) {
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
