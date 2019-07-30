using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class AppContext {
        public class CoinProfileViewModels : ViewModelBase {
            public static readonly CoinProfileViewModels Instance = new CoinProfileViewModels();

            private readonly Dictionary<Guid, CoinKernelProfileViewModel> _coinKernelProfileDicById = new Dictionary<Guid, CoinKernelProfileViewModel>();
            private readonly Dictionary<Guid, CoinProfileViewModel> _coinProfileDicById = new Dictionary<Guid, CoinProfileViewModel>();

            private CoinProfileViewModels() {
#if DEBUG
                VirtualRoot.Stopwatch.Restart();
#endif
                On<CoinKernelProfilePropertyChangedEvent>("币种内核设置变更后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_coinKernelProfileDicById.ContainsKey(message.CoinKernelId)) {
                            _coinKernelProfileDicById[message.CoinKernelId].OnPropertyChanged(message.PropertyName);
                        }
                    });
                On<CoinProfilePropertyChangedEvent>("币种设置变更后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_coinProfileDicById.ContainsKey(message.CoinId)) {
                            _coinProfileDicById[message.CoinId].OnPropertyChanged(message.PropertyName);
                        }
                    });
                VirtualRoot.On<LocalContextReInitedEvent>("LocalContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _coinKernelProfileDicById.Clear();
                        _coinProfileDicById.Clear();
                    });
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            private readonly object _coinProfileDicLocker = new object();
            public CoinProfileViewModel GetOrCreateCoinProfile(Guid coinId) {
                CoinProfileViewModel coinProfile;
                if (!_coinProfileDicById.TryGetValue(coinId, out coinProfile)) {
                    lock (_coinProfileDicLocker) {
                        if (!_coinProfileDicById.TryGetValue(coinId, out coinProfile)) {
                            coinProfile = new CoinProfileViewModel(NTMinerRoot.Instance.MinerProfile.GetCoinProfile(coinId));
                            _coinProfileDicById.Add(coinId, coinProfile);
                        }
                    }
                }
                return coinProfile;
            }

            private readonly object _coinKernelProfileLocker = new object();
            public CoinKernelProfileViewModel GetOrCreateCoinKernelProfileVm(Guid coinKernelId) {
                CoinKernelProfileViewModel coinKernelProfileVm;
                if (!_coinKernelProfileDicById.TryGetValue(coinKernelId, out coinKernelProfileVm)) {
                    lock (_coinKernelProfileLocker) {
                        if (!_coinKernelProfileDicById.TryGetValue(coinKernelId, out coinKernelProfileVm)) {
                            coinKernelProfileVm = new CoinKernelProfileViewModel(NTMinerRoot.Instance.MinerProfile.GetCoinKernelProfile(coinKernelId));
                            _coinKernelProfileDicById.Add(coinKernelId, coinKernelProfileVm);
                        }
                    }
                }

                return coinKernelProfileVm;
            }
        }
    }
}
