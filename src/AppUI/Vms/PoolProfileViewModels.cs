using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class PoolProfileViewModels : ViewModelBase {
        public static readonly PoolProfileViewModels Current = new PoolProfileViewModels();
        private readonly Dictionary<Guid, PoolProfileViewModel> _poolProfileDicById = new Dictionary<Guid, PoolProfileViewModel>();

        private PoolProfileViewModels() {
            Global.Access<PoolProfilePropertyChangedEvent>(
                Guid.Parse("EC4B0EAE-E8BA-48DA-B6FA-749A5346A669"),
                "矿池设置变更后刷新VM内存",
                LogEnum.Log,
                action: message => {
                    if (_poolProfileDicById.ContainsKey(message.PoolId)) {
                        _poolProfileDicById[message.PoolId].OnPropertyChanged(message.PropertyName);
                    }
                });
        }

        public PoolProfileViewModel GetOrCreatePoolProfile(Guid poolId, string userName, string password) {
            if (_poolProfileDicById.ContainsKey(poolId)) {
                return _poolProfileDicById[poolId];
            }
            PoolProfileViewModel poolProfile = new PoolProfileViewModel(NTMinerRoot.Current.PoolProfileSet.GetPoolProfile(poolId));
            _poolProfileDicById.Add(poolId, poolProfile);
            return poolProfile;
        }
    }
}
