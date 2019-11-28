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
                Write.Stopwatch.Start();
#endif
                BuildEventPath<CoinKernelProfilePropertyChangedEvent>("币种内核设置变更后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_coinKernelProfileDicById.ContainsKey(message.CoinKernelId)) {
                            _coinKernelProfileDicById[message.CoinKernelId].OnPropertyChanged(message.PropertyName);
                        }
                    });
                BuildEventPath<CoinProfilePropertyChangedEvent>("币种设置变更后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_coinProfileDicById.ContainsKey(message.CoinId)) {
                            _coinProfileDicById[message.CoinId].OnPropertyChanged(message.PropertyName);
                        }
                    });
                VirtualRoot.AddEventPath<LocalContextReInitedEvent>("LocalContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _coinKernelProfileDicById.Clear();
                        _coinProfileDicById.Clear();
                    });
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
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
