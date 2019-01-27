using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class SysDicItemViewModels : ViewModelBase {
        public static readonly SysDicItemViewModels Current = new SysDicItemViewModels();

        private readonly Dictionary<Guid, SysDicItemViewModel> _dicById = new Dictionary<Guid, SysDicItemViewModel>();

        public SysDicItemViewModels() {
            Global.Access<SysDicItemSetRefreshedEvent>(
                Guid.Parse("ECED5930-39A0-4422-A217-34469B600D5A"),
                "系统字典项数据集刷新后刷新Vm内存",
                LogEnum.Console,
                action: message => {
                    Init(isRefresh: true);
                });
            Global.Access<SysDicItemAddedEvent>(
                Guid.Parse("3527e754-9b63-4931-8b14-5b5cada26165"),
                "添加了系统字典项后调整VM内存",
                LogEnum.Console,
                action: (message) => {
                    if (!_dicById.ContainsKey(message.Source.GetId())) {
                        _dicById.Add(message.Source.GetId(), new SysDicItemViewModel(message.Source));
                        OnPropertyChanged(nameof(List));
                        OnPropertyChanged(nameof(Count));
                        SysDicViewModel sysDicVm;
                        if (SysDicViewModels.Current.TryGetSysDicVm(message.Source.DicId, out sysDicVm)) {
                            sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItems));
                            sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItemsSelect));
                        }
                    }
                });
            Global.Access<SysDicItemUpdatedEvent>(
                Guid.Parse("9146e461-dc8f-4aba-9254-6b81fe79389e"),
                "更新了系统字典项后调整VM内存",
                LogEnum.Console,
                action: (message) => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        SysDicItemViewModel entity = _dicById[message.Source.GetId()];
                        int sortNumber = entity.SortNumber;
                        entity.Update(message.Source);
                        if (sortNumber != entity.SortNumber) {
                            SysDicViewModel sysDicVm;
                            if (SysDicViewModels.Current.TryGetSysDicVm(entity.DicId, out sysDicVm)) {
                                sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItems));
                                sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItemsSelect));
                            }
                        }
                    }
                });
            Global.Access<SysDicItemRemovedEvent>(
                Guid.Parse("767cf0bd-f645-43f6-984d-0bde96786837"),
                "删除了系统字典项后调整VM内存",
                LogEnum.Console,
                action: (message) => {
                    _dicById.Remove(message.Source.GetId());
                    OnPropertyChanged(nameof(List));
                    OnPropertyChanged(nameof(Count));
                    SysDicViewModel sysDicVm;
                    if (SysDicViewModels.Current.TryGetSysDicVm(message.Source.DicId, out sysDicVm)) {
                        sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItems));
                        sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItemsSelect));
                    }
                });
            Init();
        }

        private void Init(bool isRefresh = false) {
            if (isRefresh) {
                foreach (var item in NTMinerRoot.Current.SysDicItemSet) {
                    SysDicItemViewModel vm;
                    if (_dicById.TryGetValue(item.GetId(), out vm)) {
                        Global.Execute(new UpdateSysDicItemCommand(item));
                    }
                    else {
                        Global.Execute(new AddSysDicItemCommand(item));
                    }
                }
            }
            else {
                foreach (var item in NTMinerRoot.Current.SysDicItemSet) {
                    _dicById.Add(item.GetId(), new SysDicItemViewModel(item));
                }
            }
        }

        public int Count {
            get {
                return _dicById.Count;
            }
        }

        public List<SysDicItemViewModel> List {
            get {
                return _dicById.Values.ToList();
            }
        }
    }
}
