using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class KernelOutputKeywordViewModels : ViewModelBase {
            public static KernelOutputKeywordViewModels Instance { get; private set; } = new KernelOutputKeywordViewModels();
            private readonly Dictionary<Guid, List<KernelOutputKeywordViewModel>> _dicByKernelOutputId = new Dictionary<Guid, List<KernelOutputKeywordViewModel>>();
            private readonly Dictionary<Guid, KernelOutputKeywordViewModel> _dicById = new Dictionary<Guid, KernelOutputKeywordViewModel>();

            private KernelOutputKeywordViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                BuildEventPath<KernelOutputKeywordLoadedEvent>("从服务器加载了内核输入关键字后刷新Vm集", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        KernelOutputKeywordViewModel[] toRemoves = _dicById.Where(a => a.Value.GetDataLevel() == DataLevel.Global).Select(a => a.Value).ToArray();
                        foreach (var item in toRemoves) {
                            _dicById.Remove(item.Id);
                        }
                        foreach (var kv in _dicByKernelOutputId) {
                            foreach (var toRemove in toRemoves.Where(a => a.KernelOutputId == kv.Key)) {
                                kv.Value.Remove(toRemove);
                            }
                        }
                        if (message.Data != null && message.Data.Count != 0) {
                            foreach (var item in message.Data) {
                                var vm = new KernelOutputKeywordViewModel(item);
                                if (!_dicById.ContainsKey(item.Id)) {
                                    _dicById.Add(item.Id, vm);
                                }
                                if (!_dicByKernelOutputId.ContainsKey(item.KernelOutputId)) {
                                    _dicByKernelOutputId.Add(item.KernelOutputId, new List<KernelOutputKeywordViewModel>());
                                }
                                _dicByKernelOutputId[item.KernelOutputId].Add(vm);
                            }
                        }
                        // TODO:给事件处理程序引入执行顺序改变，引入几个典型的顺序后可以避免很多为了顺序而构建的事件类型
                        if (NTMinerContext.Instance.CurrentMineContext != null) {
                            if (KernelOutputVms.TryGetKernelOutputVm(NTMinerContext.Instance.CurrentMineContext.KernelOutput.GetId(), out KernelOutputViewModel kernelOutputVm)) {
                                kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputKeywords));
                            }
                        }
                    });
                BuildEventPath<UserKernelOutputKeywordAddedEvent>("添加了内核输出过滤器后刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            KernelOutputKeywordViewModel vm = new KernelOutputKeywordViewModel(message.Source);
                            _dicById.Add(vm.Id, vm);
                            if (KernelOutputVms.TryGetKernelOutputVm(vm.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                                if (!_dicByKernelOutputId.ContainsKey(vm.KernelOutputId)) {
                                    _dicByKernelOutputId.Add(vm.KernelOutputId, new List<KernelOutputKeywordViewModel>());
                                }
                                _dicByKernelOutputId[vm.KernelOutputId].Add(vm);
                                kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputKeywords));
                            }
                        }
                    });
                BuildEventPath<UserKernelOutputKeywordUpdatedEvent>("更新了内核输出过滤器后刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out KernelOutputKeywordViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    });
                BuildEventPath<UserKernelOutputKeywordRemovedEvent>("删除了内核输出过滤器后刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out KernelOutputKeywordViewModel vm)) {
                            _dicById.Remove(vm.Id);
                            _dicByKernelOutputId[vm.KernelOutputId].Remove(vm);
                            if (KernelOutputVms.TryGetKernelOutputVm(vm.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                                kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputKeywords));
                            }
                        }
                    });
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.KernelOutputKeywordSet.AsEnumerable()) {
                    var vm = new KernelOutputKeywordViewModel(item);
                    if (!_dicById.ContainsKey(item.GetId())) {
                        _dicById.Add(item.GetId(), vm);
                    }
                    if (!_dicByKernelOutputId.ContainsKey(item.KernelOutputId)) {
                        _dicByKernelOutputId.Add(item.KernelOutputId, new List<KernelOutputKeywordViewModel>());
                    }
                    _dicByKernelOutputId[item.KernelOutputId].Add(vm);
                }
            }

            public IEnumerable<KernelOutputKeywordViewModel> GetListByKernelOutputId(Guid kernelOutputId) {
                if (_dicByKernelOutputId.ContainsKey(kernelOutputId)) {
                    return _dicByKernelOutputId[kernelOutputId];
                }
                return new List<KernelOutputKeywordViewModel>();
            }
        }
    }
}
