using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class PoolProfileViewModels : ViewModelBase {
        public static readonly PoolProfileViewModels Current = new PoolProfileViewModels();
        private readonly Dictionary<Guid, PoolProfileViewModel> _poolProfileDicById = new Dictionary<Guid, PoolProfileViewModel>();

        private PoolProfileViewModels() {
            VirtualRoot.On<PoolProfilePropertyChangedEvent>(
                "矿池设置变更后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (_poolProfileDicById.ContainsKey(message.PoolId)) {
                        _poolProfileDicById[message.PoolId].OnPropertyChanged(message.PropertyName);
                    }
                });
            VirtualRoot.On<MinerProfileSwichedEvent>(
                "MinerProfile切换后刷新Vm内存",
                LogEnum.Console,
                action: message => {
                    _poolProfileDicById.Clear();
                });
        }

        public PoolProfileViewModel GetOrCreatePoolProfile(Guid poolId) {
            if (_poolProfileDicById.ContainsKey(poolId)) {
                return _poolProfileDicById[poolId];
            }
            PoolProfileViewModel poolProfile = new PoolProfileViewModel(NTMinerRoot.Current.MinerProfile.GetPoolProfile(poolId));
            _poolProfileDicById.Add(poolId, poolProfile);
            return poolProfile;
        }
    }
}
