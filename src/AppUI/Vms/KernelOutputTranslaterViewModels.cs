using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class KernelOutputTranslaterViewModels : ViewModelBase {
        public static readonly KernelOutputTranslaterViewModels Current = new KernelOutputTranslaterViewModels();

        private readonly Dictionary<Guid, List<KernelOutputTranslaterViewModel>> _dicByKernelId = new Dictionary<Guid, List<KernelOutputTranslaterViewModel>>();
        private readonly Dictionary<Guid, KernelOutputTranslaterViewModel> _dicById = new Dictionary<Guid, KernelOutputTranslaterViewModel>();

        private KernelOutputTranslaterViewModels() {
            foreach (var item in NTMinerRoot.Current.KernelOutputTranslaterSet) {
                if (!_dicByKernelId.ContainsKey(item.KernelId)) {
                    _dicByKernelId.Add(item.KernelId, new List<KernelOutputTranslaterViewModel>());
                }
                var vm = new KernelOutputTranslaterViewModel(item);
                _dicByKernelId[item.KernelId].Add(vm);
                _dicById.Add(item.GetId(), vm);
            }
            Global.Access<KernelOutputTranslaterAddedEvent>(
                Guid.Parse("70f5bc18-3536-4306-9af7-256f323c9313"),
                "添加了内核输出翻译器后刷新VM内存",
                LogEnum.None,
                action: message => {
                    KernelViewModel kernelVm;
                    if (KernelViewModels.Current.TryGetKernelVm(message.Source.KernelId, out kernelVm)) {
                        if (!_dicByKernelId.ContainsKey(message.Source.KernelId)) {
                            _dicByKernelId.Add(message.Source.KernelId, new List<KernelOutputTranslaterViewModel>());
                        }
                        var vm = new KernelOutputTranslaterViewModel(message.Source);
                        _dicByKernelId[message.Source.KernelId].Add(vm);
                        _dicById.Add(message.Source.GetId(), vm);
                        kernelVm.OnPropertyChanged(nameof(kernelVm.KernelOutputTranslaters));
                    }
                });
            Global.Access<KernelOutputTranslaterUpdatedEvent>(
                Guid.Parse("eef26e4b-af61-436b-9f24-9e128d614598"),
                "更新了内核输出翻译器后刷新VM内存",
                LogEnum.None,
                action: message => {
                    if (_dicByKernelId.ContainsKey(message.Source.KernelId)) {
                        var item = _dicByKernelId[message.Source.KernelId].FirstOrDefault(a => a.Id == message.Source.GetId());
                        if (item != null) {
                            item.Update(message.Source);
                        }
                    }
                });
            Global.Access<KernelOutputTranslaterRemovedEvent>(
                Guid.Parse("d77c3aaa-be1f-41b2-9e9f-495fa6a076bf"),
                "移除了内核输出翻译器后刷新VM内存",
                LogEnum.None,
                action: message => {
                    if (_dicByKernelId.ContainsKey(message.Source.KernelId)) {
                        var item = _dicByKernelId[message.Source.KernelId].FirstOrDefault(a => a.Id == message.Source.GetId());
                        if (item != null) {
                            _dicByKernelId[message.Source.KernelId].Remove(item);
                        }
                    }
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        _dicById.Remove(message.Source.GetId());
                    }
                    KernelViewModel kernelVm;
                    if (KernelViewModels.Current.TryGetKernelVm(message.Source.KernelId, out kernelVm)) {
                        kernelVm.OnPropertyChanged(nameof(kernelVm.KernelOutputTranslaters));
                    }
                });
        }

        public IEnumerable<KernelOutputTranslaterViewModel> AllKernelOutputTranslaterVms {
            get {
                return _dicById.Values;
            }
        }

        public IEnumerable<KernelOutputTranslaterViewModel> GetListByKernelId(Guid kernelId) {
            if (_dicByKernelId.ContainsKey(kernelId)) {
                return _dicByKernelId[kernelId];
            }
            return new List<KernelOutputTranslaterViewModel>();
        }
    }
}
