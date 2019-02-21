using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class PoolProfileViewModels : ViewModelBase {
        public static readonly PoolProfileViewModels Current = new PoolProfileViewModels();
        private readonly Dictionary<Guid, PoolProfileViewModel> _poolProfileDicById = new Dictionary<Guid, PoolProfileViewModel>();

        private PoolProfileViewModels() {
            VirtualRoot.On<PoolProfilePropertyChangedEvent>(
                Guid.Parse("EC4B0EAE-E8BA-48DA-B6FA-749A5346A669"),
                "矿池设置变更后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (_poolProfileDicById.ContainsKey(message.PoolId)) {
                        _poolProfileDicById[message.PoolId].OnPropertyChanged(message.PropertyName);
                    }
                });
            VirtualRoot.On<MinerProfileSwichedEvent>(
                Guid.Parse("AE442E71-BB88-4A13-AA3F-E0C65429EC49"),
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
