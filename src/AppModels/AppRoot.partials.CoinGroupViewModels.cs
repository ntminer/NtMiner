using NTMiner.Vms;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static partial class AppRoot {
        public class CoinGroupViewModels : ViewModelBase {
            public static readonly CoinGroupViewModels Instance = new CoinGroupViewModels();

            private readonly Dictionary<Guid, CoinGroupViewModel> _dicById = new Dictionary<Guid, CoinGroupViewModel>();
            private readonly Dictionary<Guid, List<CoinGroupViewModel>> _listByGroupId = new Dictionary<Guid, List<CoinGroupViewModel>>();
            private CoinGroupViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
#if DEBUG
                NTStopwatch.Start();
#endif
                VirtualRoot.AddEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        _listByGroupId.Clear();
                        Init();
                    }, location: this.GetType());
                AddEventPath<CoinGroupAddedEvent>("添加了币组后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (!_dicById.ContainsKey(message.Target.GetId())) {
                            CoinGroupViewModel coinGroupVm = new CoinGroupViewModel(message.Target);
                            _dicById.Add(message.Target.GetId(), coinGroupVm);
                            if (!_listByGroupId.ContainsKey(coinGroupVm.GroupId)) {
                                _listByGroupId.Add(coinGroupVm.GroupId, new List<CoinGroupViewModel>());
                            }
                            _listByGroupId[coinGroupVm.GroupId].Add(coinGroupVm);
                            OnGroupPropertyChanged(coinGroupVm.GroupId);
                        }
                    }, location: this.GetType());
                AddEventPath<CoinGroupRemovedEvent>("删除了币组后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.ContainsKey(message.Target.GetId())) {
                            var entity = _dicById[message.Target.GetId()];
                            _dicById.Remove(message.Target.GetId());
                            if (_listByGroupId.ContainsKey(entity.GroupId)) {
                                _listByGroupId[entity.GroupId].Remove(entity);
                            }
                            OnGroupPropertyChanged(entity.GroupId);
                        }
                    }, location: this.GetType());
                Init();
#if DEBUG
                var elapsedMilliseconds = NTStopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.ServerContext.CoinGroupSet.AsEnumerable()) {
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
