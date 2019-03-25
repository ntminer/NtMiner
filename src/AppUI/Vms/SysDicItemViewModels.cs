using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class SysDicItemViewModels : ViewModelBase {
        public static readonly SysDicItemViewModels Current = new SysDicItemViewModels();

        private readonly Dictionary<Guid, SysDicItemViewModel> _dicById = new Dictionary<Guid, SysDicItemViewModel>();

        public SysDicItemViewModels() {
            NTMinerRoot.Current.OnContextReInited += () => {
                _dicById.Clear();
                Init();
            };
            NTMinerRoot.Current.OnReRendContext += () => {
                AllPropertyChanged();
            };
            Init();
        }

        private void Init() {
            VirtualRoot.On<SysDicItemAddedEvent>("添加了系统字典项后调整VM内存", LogEnum.DevConsole,
                action: (message) => {
                    if (!_dicById.ContainsKey(message.Source.GetId())) {
                        _dicById.Add(message.Source.GetId(), new SysDicItemViewModel(message.Source));
                        OnPropertyChanged(nameof(List));
                        OnPropertyChanged(nameof(Count));
                        OnPropertyChanged(nameof(KernelBrandItems));
                        SysDicViewModel sysDicVm;
                        if (SysDicViewModels.Current.TryGetSysDicVm(message.Source.DicId, out sysDicVm)) {
                            sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItems));
                            sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItemsSelect));
                        }
                    }
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            VirtualRoot.On<SysDicItemUpdatedEvent>("更新了系统字典项后调整VM内存", LogEnum.DevConsole,
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
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            VirtualRoot.On<SysDicItemRemovedEvent>("删除了系统字典项后调整VM内存", LogEnum.DevConsole,
                action: (message) => {
                    _dicById.Remove(message.Source.GetId());
                    OnPropertyChanged(nameof(List));
                    OnPropertyChanged(nameof(Count));
                    OnPropertyChanged(nameof(KernelBrandItems));
                    SysDicViewModel sysDicVm;
                    if (SysDicViewModels.Current.TryGetSysDicVm(message.Source.DicId, out sysDicVm)) {
                        sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItems));
                        sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItemsSelect));
                    }
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            foreach (var item in NTMinerRoot.Current.SysDicItemSet) {
                _dicById.Add(item.GetId(), new SysDicItemViewModel(item));
            }
        }

        public List<SysDicItemViewModel> KernelBrandItems {
            get {
                List<SysDicItemViewModel> list = new List<SysDicItemViewModel> {
                    SysDicItemViewModel.PleaseSelect
                };
                SysDicViewModel sysDic;
                if (SysDicViewModels.Current.TryGetSysDicVm("KernelBrand", out sysDic)) {
                    list.AddRange(List.Where(a => a.DicId == sysDic.Id));
                }
                return list;
            }
        }

        public int Count {
            get {
                return _dicById.Count;
            }
        }

        public bool TryGetValue(Guid id, out SysDicItemViewModel vm) {
            return _dicById.TryGetValue(id, out vm);
        }

        public List<SysDicItemViewModel> List {
            get {
                return _dicById.Values.ToList();
            }
        }
    }
}
