using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class CoinProfileViewModels : ViewModelBase {
        public static readonly CoinProfileViewModels Current = new CoinProfileViewModels();

        private readonly Dictionary<Guid, CoinKernelProfileViewModel> _coinKernelProfileDicById = new Dictionary<Guid, CoinKernelProfileViewModel>();
        private readonly Dictionary<Guid, CoinProfileViewModel> _coinProfileDicById = new Dictionary<Guid, CoinProfileViewModel>();

        private CoinProfileViewModels() {
            VirtualRoot.Access<CoinKernelProfilePropertyChangedEvent>(
                Guid.Parse("78C01B0B-4569-496B-92E7-009E577FE695"),
                "币种内核设置变更后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (_coinKernelProfileDicById.ContainsKey(message.CoinKernelId)) {
                        _coinKernelProfileDicById[message.CoinKernelId].OnPropertyChanged(message.PropertyName);
                    }
                });
            VirtualRoot.Access<CoinProfilePropertyChangedEvent>(
                Guid.Parse("5784E496-14B4-49BD-9234-D2793CB679A5"),
                "币种设置变更后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (_coinProfileDicById.ContainsKey(message.CoinId)) {
                        _coinProfileDicById[message.CoinId].OnPropertyChanged(message.PropertyName);
                    }
                });
        }

        public CoinProfileViewModel GetOrCreateCoinProfile(Guid coinId) {
            if (_coinProfileDicById.ContainsKey(coinId)) {
                return _coinProfileDicById[coinId];
            }
            CoinProfileViewModel coinProfile = new CoinProfileViewModel(NTMinerRoot.Current.CoinProfileSet.GetCoinProfile(coinId));
            _coinProfileDicById.Add(coinId, coinProfile);
            return coinProfile;
        }

        public CoinKernelProfileViewModel GetOrCreateCoinKernelProfileVm(Guid coinKernelId) {
            if (_coinKernelProfileDicById.ContainsKey(coinKernelId)) {
                return _coinKernelProfileDicById[coinKernelId];
            }
            CoinKernelProfileViewModel coinKernelProfileViewModel = new CoinKernelProfileViewModel(NTMinerRoot.Current.CoinKernelProfileSet.GetCoinKernelProfile(coinKernelId));
            _coinKernelProfileDicById.Add(coinKernelId, coinKernelProfileViewModel);
            return coinKernelProfileViewModel;
        }
    }
}
