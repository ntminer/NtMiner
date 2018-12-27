using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class KernelOutputFilterViewModels : ViewModelBase {
        public static readonly KernelOutputFilterViewModels Current = new KernelOutputFilterViewModels();

        private readonly Dictionary<Guid, List<KernelOutputFilterViewModel>> _dicByKernelId = new Dictionary<Guid, List<KernelOutputFilterViewModel>>();

        private KernelOutputFilterViewModels() {
            foreach (var item in NTMinerRoot.Current.KernelOutputFilterSet) {
                if (!_dicByKernelId.ContainsKey(item.KernelId)) {
                    _dicByKernelId.Add(item.KernelId, new List<KernelOutputFilterViewModel>());
                }
                _dicByKernelId[item.KernelId].Add(new KernelOutputFilterViewModel(item));
            }
            Global.Access<KernelOutputFilterAddedEvent>(
                Guid.Parse("d7a72ffc-ad5d-4862-b502-bffb3a9f0234"),
                "添加了内核输出过滤器后刷新VM内存",
                LogEnum.None,
                action: message => {
                    KernelViewModel kernelVm;
                    if (KernelViewModels.Current.TryGetKernelVm(message.Source.KernelId, out kernelVm)) {
                        if (!_dicByKernelId.ContainsKey(message.Source.KernelId)) {
                            _dicByKernelId.Add(message.Source.KernelId, new List<KernelOutputFilterViewModel>());
                        }
                        _dicByKernelId[message.Source.KernelId].Add(new KernelOutputFilterViewModel(message.Source));
                        kernelVm.OnPropertyChanged(nameof(kernelVm.KernelOutputFilters));
                    }
                });
            Global.Access<KernelOutputFilterUpdatedEvent>(
                Guid.Parse("0439daab-5248-4897-a156-1adf3f2677b2"),
                "更新了内核输出过滤器后刷新VM内存",
                LogEnum.None,
                action: message => {
                    if (_dicByKernelId.ContainsKey(message.Source.KernelId)) {
                        var item = _dicByKernelId[message.Source.KernelId].FirstOrDefault(a => a.Id == message.Source.GetId());
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
                    if (_dicByKernelId.ContainsKey(message.Source.KernelId)) {
                        var item = _dicByKernelId[message.Source.KernelId].FirstOrDefault(a => a.Id == message.Source.GetId());
                        if (item != null) {
                            _dicByKernelId[message.Source.KernelId].Remove(item);
                        }
                    }
                    KernelViewModel kernelVm;
                    if (KernelViewModels.Current.TryGetKernelVm(message.Source.KernelId, out kernelVm)) {
                        kernelVm.OnPropertyChanged(nameof(kernelVm.KernelOutputFilters));
                    }
                });
        }

        public IEnumerable<KernelOutputFilterViewModel> GetListByKernelId(Guid kernelId) {
            if (_dicByKernelId.ContainsKey(kernelId)) {
                return _dicByKernelId[kernelId];
            }
            return new List<KernelOutputFilterViewModel>();
        }
    }
}
