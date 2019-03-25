using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class CoinProfileViewModels : ViewModelBase {
        public static readonly CoinProfileViewModels Current = new CoinProfileViewModels();

        private readonly Dictionary<Guid, CoinKernelProfileViewModel> _coinKernelProfileDicById = new Dictionary<Guid, CoinKernelProfileViewModel>();
        private readonly Dictionary<Guid, CoinProfileViewModel> _coinProfileDicById = new Dictionary<Guid, CoinProfileViewModel>();

        private CoinProfileViewModels() {
            VirtualRoot.On<CoinKernelProfilePropertyChangedEvent>("币种内核设置变更后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    if (_coinKernelProfileDicById.ContainsKey(message.CoinKernelId)) {
                        _coinKernelProfileDicById[message.CoinKernelId].OnPropertyChanged(message.PropertyName);
                    }
                });
            VirtualRoot.On<CoinProfilePropertyChangedEvent>("币种设置变更后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    if (_coinProfileDicById.ContainsKey(message.CoinId)) {
                        _coinProfileDicById[message.CoinId].OnPropertyChanged(message.PropertyName);
                    }
                });
            NTMinerRoot.Current.OnMinerProfileReInited += ()=> {
                _coinKernelProfileDicById.Clear();
                _coinProfileDicById.Clear();
            };
        }

        private readonly object _coinProfileDicLocker = new object();
        public CoinProfileViewModel GetOrCreateCoinProfile(Guid coinId) {
            CoinProfileViewModel coinProfile;
            if (!_coinProfileDicById.TryGetValue(coinId, out coinProfile)) {
                lock (_coinProfileDicLocker) {
                    if (!_coinProfileDicById.TryGetValue(coinId, out coinProfile)) {
                        coinProfile = new CoinProfileViewModel(NTMinerRoot.Current.MinerProfile.GetCoinProfile(coinId));
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
                        coinKernelProfileVm = new CoinKernelProfileViewModel(NTMinerRoot.Current.MinerProfile.GetCoinKernelProfile(coinKernelId));
                        _coinKernelProfileDicById.Add(coinKernelId, coinKernelProfileVm);
                    }
                }
            }
            
            return coinKernelProfileVm;
        }
    }
}
