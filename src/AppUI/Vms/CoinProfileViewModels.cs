using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class CoinProfileViewModels : ViewModelBase {
        public static readonly CoinProfileViewModels Current = new CoinProfileViewModels();

        private readonly Dictionary<Guid, CoinKernelProfileViewModel> _coinKernelProfileDicById = new Dictionary<Guid, CoinKernelProfileViewModel>();
        private readonly Dictionary<Guid, CoinProfileViewModel> _coinProfileDicById = new Dictionary<Guid, CoinProfileViewModel>();

        private CoinProfileViewModels() {
            VirtualRoot.On<CoinKernelProfilePropertyChangedEvent>(
                "币种内核设置变更后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (_coinKernelProfileDicById.ContainsKey(message.CoinKernelId)) {
                        _coinKernelProfileDicById[message.CoinKernelId].OnPropertyChanged(message.PropertyName);
                    }
                });
            VirtualRoot.On<CoinProfilePropertyChangedEvent>(
                "币种设置变更后刷新VM内存",
                LogEnum.Console,
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

        public CoinProfileViewModel GetOrCreateCoinProfile(Guid coinId) {
            if (_coinProfileDicById.ContainsKey(coinId)) {
                return _coinProfileDicById[coinId];
            }
            CoinProfileViewModel coinProfile = new CoinProfileViewModel(NTMinerRoot.Current.MinerProfile.GetCoinProfile(coinId));
            _coinProfileDicById.Add(coinId, coinProfile);
            return coinProfile;
        }

        public CoinKernelProfileViewModel GetOrCreateCoinKernelProfileVm(Guid coinKernelId) {
            if (_coinKernelProfileDicById.ContainsKey(coinKernelId)) {
                return _coinKernelProfileDicById[coinKernelId];
            }
            CoinKernelProfileViewModel coinKernelProfileViewModel = new CoinKernelProfileViewModel(NTMinerRoot.Current.MinerProfile.GetCoinKernelProfile(coinKernelId));
            _coinKernelProfileDicById.Add(coinKernelId, coinKernelProfileViewModel);
            return coinKernelProfileViewModel;
        }
    }
}
