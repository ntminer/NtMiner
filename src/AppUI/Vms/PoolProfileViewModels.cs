using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class PoolProfileViewModels : ViewModelBase {
        public static readonly PoolProfileViewModels Current = new PoolProfileViewModels();
        private readonly Dictionary<Guid, PoolProfileViewModel> _dicById = new Dictionary<Guid, PoolProfileViewModel>();

        private PoolProfileViewModels() {
            VirtualRoot.On<PoolProfilePropertyChangedEvent>("矿池设置变更后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    if (_dicById.TryGetValue(message.PoolId, out PoolProfileViewModel vm)) {
                        vm.OnPropertyChanged(message.PropertyName);
                    }
                });
            NTMinerRoot.Current.OnMinerProfileReInited += () => {
                _dicById.Clear();
            };
        }

        private readonly object _locker = new object();
        public PoolProfileViewModel GetOrCreatePoolProfile(Guid poolId) {
            PoolProfileViewModel poolProfile;
            if (!_dicById.TryGetValue(poolId, out poolProfile)) {
                lock (_locker) {
                    if (!_dicById.TryGetValue(poolId, out poolProfile)) {
                        poolProfile = new PoolProfileViewModel(NTMinerRoot.Current.MinerProfile.GetPoolProfile(poolId));
                        _dicById.Add(poolId, poolProfile);
                    }
                }
            }
            return poolProfile;
        }
    }
}
