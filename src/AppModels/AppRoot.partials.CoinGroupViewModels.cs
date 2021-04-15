using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class CoinGroupViewModels : ViewModelBase {
            public static CoinGroupViewModels Instance { get; private set; } = new CoinGroupViewModels();

            private readonly Dictionary<Guid, CoinGroupViewModel> _dicById = new Dictionary<Guid, CoinGroupViewModel>();
            private readonly Dictionary<Guid, List<CoinGroupViewModel>> _listByGroupId = new Dictionary<Guid, List<CoinGroupViewModel>>();
            private CoinGroupViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        _dicById.Clear();
                        _listByGroupId.Clear();
                        Init();
                    });
                VirtualRoot.BuildEventPath<ServerContextReInitedEventHandledEvent>("刷新视图界面", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        // 什么也不做，因为该集合没有什么属性
                    });
                BuildEventPath<CoinGroupAddedEvent>("调整VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            CoinGroupViewModel coinGroupVm = new CoinGroupViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), coinGroupVm);
                            if (!_listByGroupId.ContainsKey(coinGroupVm.GroupId)) {
                                _listByGroupId.Add(coinGroupVm.GroupId, new List<CoinGroupViewModel>());
                            }
                            _listByGroupId[coinGroupVm.GroupId].Add(coinGroupVm);
                            OnGroupPropertyChanged(coinGroupVm.GroupId);
                        }
                    });
                BuildEventPath<CoinGroupRemovedEvent>("调整VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: (message) => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            var entity = _dicById[message.Source.GetId()];
                            _dicById.Remove(message.Source.GetId());
                            if (_listByGroupId.ContainsKey(entity.GroupId)) {
                                _listByGroupId[entity.GroupId].Remove(entity);
                            }
                            OnGroupPropertyChanged(entity.GroupId);
                        }
                    });
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.ServerContext.CoinGroupSet.AsEnumerable().ToArray()) {
                    CoinGroupViewModel groupVm = new CoinGroupViewModel(item);
                    _dicById.Add(item.GetId(), groupVm);
                    if (!_listByGroupId.ContainsKey(item.GroupId)) {
                        _listByGroupId.Add(item.GroupId, new List<CoinGroupViewModel>());
                    }
                    _listByGroupId[item.GroupId].Add(groupVm);
                }
            }

            private void OnGroupPropertyChanged(Guid groupId) {
                if (GroupVms.TryGetGroupVm(groupId, out GroupViewModel groupVm)) {
                    groupVm.OnPropertyChanged(nameof(groupVm.CoinVms));
                    groupVm.OnPropertyChanged(nameof(groupVm.DualCoinVms));
                    groupVm.OnPropertyChanged(nameof(groupVm.CoinGroupVms));
                }
            }

            public bool TryGetGroupVm(Guid coinGroupId, out CoinGroupViewModel groupVm) {
                return _dicById.TryGetValue(coinGroupId, out groupVm);
            }

            public List<CoinGroupViewModel> GetCoinGroupsByGroupId(Guid groupId) {
                if (!_listByGroupId.ContainsKey(groupId)) {
                    return new List<CoinGroupViewModel>();
                }
                return _listByGroupId[groupId];
            }
        }
    }
}
