using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class AppContext {
        public class KernelOutputFilterViewModels : ViewModelBase {
            private readonly Dictionary<Guid, List<KernelOutputFilterViewModel>> _dicByKernelOutputId = new Dictionary<Guid, List<KernelOutputFilterViewModel>>();
            private readonly Dictionary<Guid, KernelOutputFilterViewModel> _dicById = new Dictionary<Guid, KernelOutputFilterViewModel>();

            public KernelOutputFilterViewModels() {
                NTMinerRoot.Instance.OnContextReInited += () => {
                    _dicById.Clear();
                    _dicByKernelOutputId.Clear();
                    Init();
                };
                Init();
            }

            private void Init() {
                VirtualRoot.On<KernelOutputFilterAddedEvent>("添加了内核输出过滤器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            KernelOutputFilterViewModel vm = new KernelOutputFilterViewModel(message.Source);
                            _dicById.Add(vm.Id, vm);
                            if (Current.KernelOutputVms.TryGetKernelOutputVm(vm.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                                if (!_dicByKernelOutputId.ContainsKey(vm.KernelOutputId)) {
                                    _dicByKernelOutputId.Add(vm.KernelOutputId, new List<KernelOutputFilterViewModel>());
                                }
                                _dicByKernelOutputId[vm.KernelOutputId].Add(vm);
                                kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputFilters));
                            }
                        }
                    }).AddToCollection(NTMinerRoot.Instance.ContextHandlers);
                VirtualRoot.On<KernelOutputFilterUpdatedEvent>("更新了内核输出过滤器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out KernelOutputFilterViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    }).AddToCollection(NTMinerRoot.Instance.ContextHandlers);
                VirtualRoot.On<KernelOutputFilterRemovedEvent>("删除了内核输出过滤器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out KernelOutputFilterViewModel vm)) {
                            _dicById.Remove(vm.Id);
                            _dicByKernelOutputId[vm.KernelOutputId].Remove(vm);
                            KernelOutputViewModel kernelOutputVm;
                            if (Current.KernelOutputVms.TryGetKernelOutputVm(vm.KernelOutputId, out kernelOutputVm)) {
                                kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputFilters));
                            }
                        }
                    }).AddToCollection(NTMinerRoot.Instance.ContextHandlers);
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
