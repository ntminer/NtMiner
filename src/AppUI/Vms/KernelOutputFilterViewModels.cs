using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class KernelOutputFilterViewModels : ViewModelBase {
        public static readonly KernelOutputFilterViewModels Current = new KernelOutputFilterViewModels();

        private readonly Dictionary<Guid, List<KernelOutputFilterViewModel>> _dicByKernelOutputId = new Dictionary<Guid, List<KernelOutputFilterViewModel>>();

        private KernelOutputFilterViewModels() {
            foreach (var item in NTMinerRoot.Current.KernelOutputFilterSet) {
                if (!_dicByKernelOutputId.ContainsKey(item.KernelOutputId)) {
                    _dicByKernelOutputId.Add(item.KernelOutputId, new List<KernelOutputFilterViewModel>());
                }
                _dicByKernelOutputId[item.KernelOutputId].Add(new KernelOutputFilterViewModel(item));
            }
            Global.Access<KernelOutputFilterAddedEvent>(
                Guid.Parse("d7a72ffc-ad5d-4862-b502-bffb3a9f0234"),
                "添加了内核输出过滤器后刷新VM内存",
                LogEnum.None,
                action: message => {
                    KernelOutputViewModel kernelOutputVm;
                    if (KernelOutputViewModels.Current.TryGetKernelOutputVm(message.Source.KernelOutputId, out kernelOutputVm)) {
                        if (!_dicByKernelOutputId.ContainsKey(message.Source.KernelOutputId)) {
                            _dicByKernelOutputId.Add(message.Source.KernelOutputId, new List<KernelOutputFilterViewModel>());
                        }
                        _dicByKernelOutputId[message.Source.KernelOutputId].Add(new KernelOutputFilterViewModel(message.Source));
                        kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputFilters));
                    }
                });
            Global.Access<KernelOutputFilterUpdatedEvent>(
                Guid.Parse("0439daab-5248-4897-a156-1adf3f2677b2"),
                "更新了内核输出过滤器后刷新VM内存",
                LogEnum.None,
                action: message => {
                    if (_dicByKernelOutputId.ContainsKey(message.Source.KernelOutputId)) {
                        var item = _dicByKernelOutputId[message.Source.KernelOutputId].FirstOrDefault(a => a.Id == message.Source.GetId());
                        if (item != null) {
                            item.Update(message.Source);
                        }
                    }
                });
            Global.Access<KernelOutputFilterRemovedEvent>(
                Guid.Parse("d08e92d9-0849-4e1b-8265-40ed74053667"),
                "删除了内核输出过滤器后刷新VM内存",
                LogEnum.None,
                action: message => {
                    if (_dicByKernelOutputId.ContainsKey(message.Source.KernelOutputId)) {
                        var item = _dicByKernelOutputId[message.Source.KernelOutputId].FirstOrDefault(a => a.Id == message.Source.GetId());
                        if (item != null) {
                            _dicByKernelOutputId[message.Source.KernelOutputId].Remove(item);
                        }
                    }
                    KernelOutputViewModel kernelOutputVm;
                    if (KernelOutputViewModels.Current.TryGetKernelOutputVm(message.Source.KernelOutputId, out kernelOutputVm)) {
                        kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputFilters));
                    }
                });
        }

        public IEnumerable<KernelOutputFilterViewModel> GetListByKernelId(Guid kernelId) {
            if (_dicByKernelOutputId.ContainsKey(kernelId)) {
                return _dicByKernelOutputId[kernelId];
            }
            return new List<KernelOutputFilterViewModel>();
        }
    }
}
