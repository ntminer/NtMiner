using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class AppContext {
        public class KernelOutputFilterViewModels : ViewModelBase {
            public static readonly KernelOutputFilterViewModels Instance = new KernelOutputFilterViewModels();
            private readonly Dictionary<Guid, List<KernelOutputFilterViewModel>> _dicByKernelOutputId = new Dictionary<Guid, List<KernelOutputFilterViewModel>>();
            private readonly Dictionary<Guid, KernelOutputFilterViewModel> _dicById = new Dictionary<Guid, KernelOutputFilterViewModel>();

            private KernelOutputFilterViewModels() {
#if DEBUG
                VirtualRoot.Stopwatch.Restart();
#endif
                VirtualRoot.On<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        _dicByKernelOutputId.Clear();
                        Init();
                    });
                On<KernelOutputFilterAddedEvent>("添加了内核输出过滤器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            KernelOutputFilterViewModel vm = new KernelOutputFilterViewModel(message.Source);
                            _dicById.Add(vm.Id, vm);
                            if (AppContext.Instance.KernelOutputVms.TryGetKernelOutputVm(vm.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                                if (!_dicByKernelOutputId.ContainsKey(vm.KernelOutputId)) {
                                    _dicByKernelOutputId.Add(vm.KernelOutputId, new List<KernelOutputFilterViewModel>());
                                }
                                _dicByKernelOutputId[vm.KernelOutputId].Add(vm);
                                kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputFilters));
                            }
                        }
                    });
                On<KernelOutputFilterUpdatedEvent>("更新了内核输出过滤器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out KernelOutputFilterViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    });
                On<KernelOutputFilterRemovedEvent>("删除了内核输出过滤器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out KernelOutputFilterViewModel vm)) {
                            _dicById.Remove(vm.Id);
                            _dicByKernelOutputId[vm.KernelOutputId].Remove(vm);
                            KernelOutputViewModel kernelOutputVm;
                            if (AppContext.Instance.KernelOutputVms.TryGetKernelOutputVm(vm.KernelOutputId, out kernelOutputVm)) {
                                kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputFilters));
                            }
                        }
                    });
                Init();
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.KernelOutputFilterSet) {
                    if (!_dicByKernelOutputId.ContainsKey(item.KernelOutputId)) {
                        _dicByKernelOutputId.Add(item.KernelOutputId, new List<KernelOutputFilterViewModel>());
                    }
                    _dicByKernelOutputId[item.KernelOutputId].Add(new KernelOutputFilterViewModel(item));
                }
            }

            public IEnumerable<KernelOutputFilterViewModel> GetListByKernelId(Guid kernelId) {
                if (_dicByKernelOutputId.ContainsKey(kernelId)) {
                    return _dicByKernelOutputId[kernelId];
                }
                return new List<KernelOutputFilterViewModel>();
            }
        }
    }
}
