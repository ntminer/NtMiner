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
                LogEnum.DevConsole,
                action: message => {
                    if (_poolProfileDicById.ContainsKey(message.PoolId)) {
                        _poolProfileDicById[message.PoolId].OnPropertyChanged(message.PropertyName);
                    }
                });
            NTMinerRoot.Current.OnMinerProfileReInited += () => {
                _poolProfileDicById.Clear();
            };
        }

        private readonly object _poolProfileLocker = new object();
        public PoolProfileViewModel GetOrCreatePoolProfile(Guid poolId) {
            PoolProfileViewModel poolProfile;
            if (!_poolProfileDicById.TryGetValue(poolId, out poolProfile)) {
                lock (_poolProfileLocker) {
                    if (!_poolProfileDicById.TryGetValue(poolId, out poolProfile)) {
                        poolProfile = new PoolProfileViewModel(NTMinerRoot.Current.MinerProfile.GetPoolProfile(poolId));
                        _poolProfileDicById.Add(poolId, poolProfile);
                    }
                }
            }
            return poolProfile;
        }
    }
}
