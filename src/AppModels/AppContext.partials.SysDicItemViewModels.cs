using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class SysDicItemViewModels : ViewModelBase {
            public static readonly SysDicItemViewModels Instance = new SysDicItemViewModels();
            private readonly Dictionary<Guid, SysDicItemViewModel> _dicById = new Dictionary<Guid, SysDicItemViewModel>();

            private SysDicItemViewModels() {
#if DEBUG
                Write.Stopwatch.Start();
#endif
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    });
                VirtualRoot.BuildEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChangeds();
                    });
                BuildEventPath<SysDicItemAddedEvent>("添加了系统字典项后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Add(message.Source.GetId(), new SysDicItemViewModel(message.Source));
                            OnPropertyChangeds();
                            if (AppContext.Instance.SysDicVms.TryGetSysDicVm(message.Source.DicId, out SysDicViewModel sysDicVm)) {
                                sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItems));
                                sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItemsSelect));
                            }
                        }
                    });
                BuildEventPath<SysDicItemUpdatedEvent>("更新了系统字典项后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            SysDicItemViewModel entity = _dicById[message.Source.GetId()];
                            int sortNumber = entity.SortNumber;
                            entity.Update(message.Source);
                            if (sortNumber != entity.SortNumber) {
                                if (AppContext.Instance.SysDicVms.TryGetSysDicVm(entity.DicId, out SysDicViewModel sysDicVm)) {
                                    sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItems));
                                    sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItemsSelect));
                                }
                            }
                        }
                    });
                BuildEventPath<SysDicItemRemovedEvent>("删除了系统字典项后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChangeds();
                        if (AppContext.Instance.SysDicVms.TryGetSysDicVm(message.Source.DicId, out SysDicViewModel sysDicVm)) {
                            sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItems));
                            sysDicVm.OnPropertyChanged(nameof(sysDicVm.SysDicItemsSelect));
                        }
                    });
                Init();
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.ServerContext.SysDicItemSet.AsEnumerable()) {
                    _dicById.Add(item.GetId(), new SysDicItemViewModel(item));
                }
            }

            private void OnPropertyChangeds() {
                OnPropertyChanged(nameof(List));
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(nameof(KernelBrandItems));
                OnPropertyChanged(nameof(KernelBrandsSelect));
                OnPropertyChanged(nameof(PoolBrandItems));
                OnPropertyChanged(nameof(AlgoItems));
            }

            public List<SysDicItemViewModel> KernelBrandItems {
                get {
                    List<SysDicItemViewModel> list = new List<SysDicItemViewModel>();
                    if (AppContext.Instance.SysDicVms.TryGetSysDicVm(NTKeyword.KernelBrandSysDicCode, out SysDicViewModel sysDic)) {
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
                    if (AppContext.Instance.SysDicVms.TryGetSysDicVm(NTKeyword.KernelBrandSysDicCode, out SysDicViewModel sysDic)) {
                        list.AddRange(List.Where(a => a.DicId == sysDic.Id).OrderBy(a => a.SortNumber));
                    }
                    return list;
                }
            }

            public List<SysDicItemViewModel> PoolBrandItems {
                get {
                    List<SysDicItemViewModel> list = new List<SysDicItemViewModel>();
                    if (AppContext.Instance.SysDicVms.TryGetSysDicVm(NTKeyword.PoolBrandSysDicCode, out SysDicViewModel sysDic)) {
                        list.AddRange(List.Where(a => a.DicId == sysDic.Id).OrderBy(a => a.SortNumber));
                    }
                    return list;
                }
            }

            public List<SysDicItemViewModel> AlgoItems {
                get {
                    List<SysDicItemViewModel> list = new List<SysDicItemViewModel>();
                    if (AppContext.Instance.SysDicVms.TryGetSysDicVm(NTKeyword.AlgoSysDicCode, out SysDicViewModel sysDic)) {
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

            public SysDicItemViewModel GetUpOne(int sortNumber) {
                return List.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < sortNumber);
            }

            public SysDicItemViewModel GetNextOne(int sortNumber) {
                return List.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > sortNumber);
            }
        }
    }
}
