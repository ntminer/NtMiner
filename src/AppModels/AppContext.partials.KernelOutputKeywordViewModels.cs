using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class KernelOutputKeywordViewModels : ViewModelBase {
            public static readonly KernelOutputKeywordViewModels Instance = new KernelOutputKeywordViewModels();
            private readonly Dictionary<Guid, List<KernelOutputKeywordViewModel>> _dicByKernelOutputId = new Dictionary<Guid, List<KernelOutputKeywordViewModel>>();
            private readonly Dictionary<Guid, KernelOutputKeywordViewModel> _dicById = new Dictionary<Guid, KernelOutputKeywordViewModel>();

            private KernelOutputKeywordViewModels() {
#if DEBUG
                Write.Stopwatch.Start();
#endif
                AddEventPath<KernelOutputKeywordLoadedEvent>("从服务器加载了内核输入关键字后刷新Vm集", LogEnum.DevConsole,
                    action: message => {
                        KernelOutputKeywordViewModel[] toRemoves = _dicById.Where(a => a.Value.DataLevel == DataLevel.Global).Select(a => a.Value).ToArray();
                        foreach (var item in toRemoves) {
                            _dicById.Remove(item.Id);
                        }
                        foreach (var kv in _dicByKernelOutputId) {
                            foreach (var toRemove in toRemoves.Where(a=>a.KernelOutputId == kv.Key)) {
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
                        if (NTMinerRoot.Instance.CurrentMineContext != null) {
                            if (AppContext.Instance.KernelOutputVms.TryGetKernelOutputVm(NTMinerRoot.Instance.CurrentMineContext.KernelOutput.GetId(), out KernelOutputViewModel kernelOutputVm)) {
                                kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputKeywords));
                            }
                        }
                    }, location: this.GetType());
                AddEventPath<UserKernelOutputKeywordAddedEvent>("添加了内核输出过滤器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.ContainsKey(message.Target.GetId())) {
                            KernelOutputKeywordViewModel vm = new KernelOutputKeywordViewModel(message.Target);
                            _dicById.Add(vm.Id, vm);
                            if (AppContext.Instance.KernelOutputVms.TryGetKernelOutputVm(vm.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                                if (!_dicByKernelOutputId.ContainsKey(vm.KernelOutputId)) {
                                    _dicByKernelOutputId.Add(vm.KernelOutputId, new List<KernelOutputKeywordViewModel>());
                                }
                                _dicByKernelOutputId[vm.KernelOutputId].Add(vm);
                                kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputKeywords));
                            }
                        }
                    }, location: this.GetType());
                AddEventPath<UserKernelOutputKeywordUpdatedEvent>("更新了内核输出过滤器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.Target.GetId(), out KernelOutputKeywordViewModel vm)) {
                            vm.Update(message.Target);
                        }
                    }, location: this.GetType());
                AddEventPath<UserKernelOutputKeywordRemovedEvent>("删除了内核输出过滤器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.Target.GetId(), out KernelOutputKeywordViewModel vm)) {
                            _dicById.Remove(vm.Id);
                            _dicByKernelOutputId[vm.KernelOutputId].Remove(vm);
                            if (AppContext.Instance.KernelOutputVms.TryGetKernelOutputVm(vm.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                                kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputKeywords));
                            }
                        }
                    }, location: this.GetType());
                Init();
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.KernelOutputKeywordSet.AsEnumerable()) {
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

            public IEnumerable<KernelOutputKeywordViewModel> GetListByKernelId(Guid kernelOutputId) {
                if (_dicByKernelOutputId.ContainsKey(kernelOutputId)) {
                    return _dicByKernelOutputId[kernelOutputId];
                }
                return new List<KernelOutputKeywordViewModel>();
            }
        }
    }
}
