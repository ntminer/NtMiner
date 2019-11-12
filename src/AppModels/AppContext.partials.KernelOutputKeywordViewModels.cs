using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;

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
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        _dicByKernelOutputId.Clear();
                        Init();
                    });
                BuildEventPath<KernelOutputKeywordAddedEvent>("添加了内核输出过滤器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            KernelOutputKeywordViewModel vm = new KernelOutputKeywordViewModel(message.Source);
                            _dicById.Add(vm.Id, vm);
                            if (AppContext.Instance.KernelOutputVms.TryGetKernelOutputVm(vm.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                                if (!_dicByKernelOutputId.ContainsKey(vm.KernelOutputId)) {
                                    _dicByKernelOutputId.Add(vm.KernelOutputId, new List<KernelOutputKeywordViewModel>());
                                }
                                _dicByKernelOutputId[vm.KernelOutputId].Add(vm);
                                kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputKeywords));
                            }
                        }
                    });
                BuildEventPath<KernelOutputKeywordUpdatedEvent>("更新了内核输出过滤器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out KernelOutputKeywordViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    });
                BuildEventPath<KernelOutputKeywordRemovedEvent>("删除了内核输出过滤器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out KernelOutputKeywordViewModel vm)) {
                            _dicById.Remove(vm.Id);
                            _dicByKernelOutputId[vm.KernelOutputId].Remove(vm);
                            if (AppContext.Instance.KernelOutputVms.TryGetKernelOutputVm(vm.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                                kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputKeywords));
                            }
                        }
                    });
                Init();
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                Write.DevTimeSpan($"耗时{elapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.ServerKernelOutputKeywordSet) {
                    if (!_dicByKernelOutputId.ContainsKey(item.KernelOutputId)) {
                        _dicByKernelOutputId.Add(item.KernelOutputId, new List<KernelOutputKeywordViewModel>());
                    }
                    _dicByKernelOutputId[item.KernelOutputId].Add(new KernelOutputKeywordViewModel(item));
                }
                foreach (var item in NTMinerRoot.Instance.LocalKernelOutputKeywordSet) {
                    if (!_dicByKernelOutputId.ContainsKey(item.KernelOutputId)) {
                        _dicByKernelOutputId.Add(item.KernelOutputId, new List<KernelOutputKeywordViewModel>());
                    }
                    _dicByKernelOutputId[item.KernelOutputId].Add(new KernelOutputKeywordViewModel(item));
                }
            }

            public IEnumerable<KernelOutputKeywordViewModel> GetListByKernelId(Guid kernelId) {
                if (_dicByKernelOutputId.ContainsKey(kernelId)) {
                    return _dicByKernelOutputId[kernelId];
                }
                return new List<KernelOutputKeywordViewModel>();
            }
        }
    }
}
