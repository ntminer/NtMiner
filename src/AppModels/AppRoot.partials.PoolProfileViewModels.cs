using NTMiner.Vms;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static partial class AppRoot {
        public class PoolProfileViewModels : ViewModelBase {
            public static readonly PoolProfileViewModels Instance = new PoolProfileViewModels();
            private readonly Dictionary<Guid, PoolProfileViewModel> _dicById = new Dictionary<Guid, PoolProfileViewModel>();

            private PoolProfileViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                AddEventPath<PoolProfilePropertyChangedEvent>("矿池设置变更后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.PoolId, out PoolProfileViewModel vm)) {
                            vm.OnPropertyChanged(message.PropertyName);
                        }
                    }, location: this.GetType());
                VirtualRoot.AddEventPath<LocalContextReInitedEvent>("LocalContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                    }, location: this.GetType());
            }

            private readonly object _locker = new object();
            public PoolProfileViewModel GetOrCreatePoolProfile(Guid poolId) {
                if (!_dicById.TryGetValue(poolId, out PoolProfileViewModel poolProfile)) {
                    lock (_locker) {
                        if (!_dicById.TryGetValue(poolId, out poolProfile)) {
                            poolProfile = new PoolProfileViewModel(NTMinerContext.Instance.MinerProfile.GetPoolProfile(poolId));
                            _dicById.Add(poolId, poolProfile);
                        }
                    }
                }
                return poolProfile;
            }
        }
    }
}
