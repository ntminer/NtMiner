using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class CoinGroupViewModels : ViewModelBase {
        public static readonly CoinGroupViewModels Current = new CoinGroupViewModels();

        private readonly Dictionary<Guid, CoinGroupViewModel> _dicById = new Dictionary<Guid, CoinGroupViewModel>();
        private readonly Dictionary<Guid, List<CoinGroupViewModel>> _listByGroupId = new Dictionary<Guid, List<CoinGroupViewModel>>();
        private CoinGroupViewModels() {
            foreach (var item in NTMinerRoot.Current.CoinGroupSet) {
                CoinGroupViewModel groupVm = new CoinGroupViewModel(item);
                _dicById.Add(item.GetId(), groupVm);
                if (!_listByGroupId.ContainsKey(item.GroupId)) {
                    _listByGroupId.Add(item.GroupId, new List<CoinGroupViewModel>());
                }
                _listByGroupId[item.GroupId].Add(groupVm);
            }
            Global.Access<CoinGroupAddedEvent>(
                Guid.Parse("e0476d29-0115-405e-81d1-c7fb65051c83"),
                "添加了币组后调整VM内存",
                LogEnum.Log,
                action: (message) => {
                    if (!_dicById.ContainsKey(message.Source.GetId())) {
                        CoinGroupViewModel coinGroupVm = new CoinGroupViewModel(message.Source);
                        _dicById.Add(message.Source.GetId(), coinGroupVm);
                        if (!_listByGroupId.ContainsKey(coinGroupVm.GroupId)) {
                            _listByGroupId.Add(coinGroupVm.GroupId, new List<CoinGroupViewModel>());
                        }
                        _listByGroupId[coinGroupVm.GroupId].Add(coinGroupVm);
                        GroupViewModel groupVm;
                        if (GroupViewModels.Current.TryGetGroupVm(coinGroupVm.GroupId, out groupVm)) {
                            groupVm.OnPropertyChanged(nameof(groupVm.CoinVms));
                            groupVm.OnPropertyChanged(nameof(groupVm.DualCoinVms));
                            groupVm.OnPropertyChanged(nameof(groupVm.CoinGroupVms));
                        }
                    }
                });
            Global.Access<CoinGroupUpdatedEvent>(
                Guid.Parse("c600d33a-21e3-45ad-b9b3-cfd5578885f4"),
                "更新了币组后调整VM内存",
                LogEnum.Log,
                action: (message) => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        CoinGroupViewModel entity = _dicById[message.Source.GetId()];
                        int sortNumber = entity.SortNumber;
                        entity.Update(message.Source);
                        if (sortNumber != entity.SortNumber) {
                            GroupViewModel groupVm;
                            if (GroupViewModels.Current.TryGetGroupVm(entity.GroupId, out groupVm)) {
                                groupVm.OnPropertyChanged(nameof(groupVm.CoinGroupVms));
                            }
                        }
                    }
                });
            Global.Access<CoinGroupRemovedEvent>(
                Guid.Parse("76842ab6-c1a3-4eee-b951-f25be25ec35a"),
                "删除了币组后调整VM内存",
                LogEnum.Log,
                action: (message) => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        var entity = _dicById[message.Source.GetId()];
                        _dicById.Remove(message.Source.GetId());
                        if (_listByGroupId.ContainsKey(entity.GroupId)) {
                            _listByGroupId[entity.GroupId].Remove(entity);
                        }
                        GroupViewModel groupVm;
                        if (GroupViewModels.Current.TryGetGroupVm(entity.GroupId, out groupVm)) {
                            groupVm.OnPropertyChanged(nameof(groupVm.CoinVms));
                            groupVm.OnPropertyChanged(nameof(groupVm.DualCoinVms));
                            groupVm.OnPropertyChanged(nameof(groupVm.CoinGroupVms));
                        }
                    }
                });
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
