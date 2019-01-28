using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class KernelOutputFilterViewModels : ViewModelBase {
        public static readonly KernelOutputFilterViewModels Current = new KernelOutputFilterViewModels();

        private readonly Dictionary<Guid, List<KernelOutputFilterViewModel>> _dicByKernelOutputId = new Dictionary<Guid, List<KernelOutputFilterViewModel>>();
        private readonly Dictionary<Guid, KernelOutputFilterViewModel> _dicById = new Dictionary<Guid, KernelOutputFilterViewModel>();

        private KernelOutputFilterViewModels() {
            Global.Access<KernelOutputFilterAddedEvent>(
                Guid.Parse("d7a72ffc-ad5d-4862-b502-bffb3a9f0234"),
                "添加了内核输出过滤器后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (!_dicById.ContainsKey(message.Source.GetId())) {
                        KernelOutputFilterViewModel vm = new KernelOutputFilterViewModel(message.Source);
                        _dicById.Add(vm.Id, vm);
                        KernelOutputViewModel kernelOutputVm;
                        if (KernelOutputViewModels.Current.TryGetKernelOutputVm(vm.KernelOutputId, out kernelOutputVm)) {
                            if (!_dicByKernelOutputId.ContainsKey(vm.KernelOutputId)) {
                                _dicByKernelOutputId.Add(vm.KernelOutputId, new List<KernelOutputFilterViewModel>());
                            }
                            _dicByKernelOutputId[vm.KernelOutputId].Add(vm);
                            kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputFilters));
                        }
                    }
                });
            Global.Access<KernelOutputFilterUpdatedEvent>(
                Guid.Parse("0439daab-5248-4897-a156-1adf3f2677b2"),
                "更新了内核输出过滤器后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    KernelOutputFilterViewModel vm;
                    if (_dicById.TryGetValue(message.Source.GetId(), out vm)) {
                        vm.Update(message.Source);
                    }
                });
            Global.Access<KernelOutputFilterRemovedEvent>(
                Guid.Parse("d08e92d9-0849-4e1b-8265-40ed74053667"),
                "删除了内核输出过滤器后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    KernelOutputFilterViewModel vm;
                    if (_dicById.TryGetValue(message.Source.GetId(), out vm)) {
                        _dicById.Remove(vm.Id);
                        _dicByKernelOutputId[vm.KernelOutputId].Remove(vm);
                        KernelOutputViewModel kernelOutputVm;
                        if (KernelOutputViewModels.Current.TryGetKernelOutputVm(vm.KernelOutputId, out kernelOutputVm)) {
                            kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputFilters));
                        }
                    }
                });
            Init();
        }

        private void Init() {
            foreach (var item in NTMinerRoot.Current.KernelOutputFilterSet) {
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
