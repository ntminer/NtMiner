using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class SysDicItemViewModels : ViewModelBase {
            public static SysDicItemViewModels Instance { get; private set; } = new SysDicItemViewModels();
            private readonly Dictionary<Guid, SysDicItemViewModel> _dicById = new Dictionary<Guid, SysDicItemViewModel>();

            private SysDicItemViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        _dicById.Clear();
                        Init();
                    });
                VirtualRoot.BuildEventPath<ServerContextReInitedEventHandledEvent>("刷新视图界面", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        AllPropertyChanged();
                    });
                BuildEventPath<SysDicItemAddedEvent>("调整VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Add(message.Source.GetId(), new SysDicItemViewModel(message.Source));
                            AllPropertyChanged();
                            if (SysDicVms.TryGetSysDicVm(message.Source.DicId, out SysDicViewModel sysDicVm)) {
                                sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItems));
                                sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItemsSelect));
                            }
                        }
                    });
                BuildEventPath<SysDicItemUpdatedEvent>("调整VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: (message) => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out SysDicItemViewModel vm)) {
                            int sortNumber = vm.SortNumber;
                            vm.Update(message.Source);
                            if (sortNumber != vm.SortNumber) {
                                if (SysDicVms.TryGetSysDicVm(vm.DicId, out SysDicViewModel sysDicVm)) {
                                    sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItems));
                                    sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItemsSelect));
                                }
                            }
                        }
                    });
                BuildEventPath<SysDicItemRemovedEvent>("调整VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        AllPropertyChanged();
                        if (SysDicVms.TryGetSysDicVm(message.Source.DicId, out SysDicViewModel sysDicVm)) {
                            sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItems));
                            sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItemsSelect));
                        }
                    });
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.ServerContext.SysDicItemSet.AsEnumerable().ToArray()) {
                    _dicById.Add(item.GetId(), new SysDicItemViewModel(item));
                }
            }

            public List<SysDicItemViewModel> KernelBrandItems {
                get {
                    List<SysDicItemViewModel> list = new List<SysDicItemViewModel>();
                    if (SysDicVms.TryGetSysDicVm(NTKeyword.KernelBrandSysDicCode, out SysDicViewModel sysDic)) {
                        list.AddRange(List.Where(a => a.DicId == sysDic.Id).OrderBy(a => a.SortNumber));
                    }
                    return list;
                }
            }

            public List<SysDicItemViewModel> KernelBrandsSelect {
                get {
                    List<SysDicItemViewModel> list = new List<SysDicItemViewModel> {
                        SysDicItemViewModel.PleaseSelect
                    };
                    if (SysDicVms.TryGetSysDicVm(NTKeyword.KernelBrandSysDicCode, out SysDicViewModel sysDic)) {
                        list.AddRange(List.Where(a => a.DicId == sysDic.Id).OrderBy(a => a.SortNumber));
                    }
                    return list;
                }
            }

            public List<SysDicItemViewModel> PoolBrandItems {
                get {
                    List<SysDicItemViewModel> list = new List<SysDicItemViewModel>();
                    if (SysDicVms.TryGetSysDicVm(NTKeyword.PoolBrandSysDicCode, out SysDicViewModel sysDic)) {
                        list.AddRange(List.Where(a => a.DicId == sysDic.Id).OrderBy(a => a.SortNumber));
                    }
                    return list;
                }
            }

            public List<SysDicItemViewModel> AlgoItems {
                get {
                    List<SysDicItemViewModel> list = new List<SysDicItemViewModel>();
                    if (SysDicVms.TryGetSysDicVm(NTKeyword.AlgoSysDicCode, out SysDicViewModel sysDic)) {
                        list.AddRange(List.Where(a => a.DicId == sysDic.Id).OrderBy(a => a.SortNumber));
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

            public IEnumerable<SysDicItemViewModel> GetSysDicItemVmsByDicId(Guid dicId) {
                return _dicById.Values.Where(a => a.DicId == dicId);
            }
        }
    }
}
