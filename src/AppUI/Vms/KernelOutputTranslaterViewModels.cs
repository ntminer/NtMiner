using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class KernelOutputTranslaterViewModels : ViewModelBase {
        public static readonly KernelOutputTranslaterViewModels Current = new KernelOutputTranslaterViewModels();

        private readonly Dictionary<Guid, List<KernelOutputTranslaterViewModel>> _dicByKernelOutputId = new Dictionary<Guid, List<KernelOutputTranslaterViewModel>>();
        private readonly Dictionary<Guid, KernelOutputTranslaterViewModel> _dicById = new Dictionary<Guid, KernelOutputTranslaterViewModel>();

        private KernelOutputTranslaterViewModels() {
            Global.Access<KernelOutputTranslaterSetRefreshedEvent>(
                Guid.Parse("C12F9F5E-9AF9-4A59-8202-9DBB2F8EF6F8"),
                "内核输出翻译器数据集刷新后刷新Vm内存",
                LogEnum.Console,
                action: message => {
                    Init(isRefresh: true);
                });
            Global.Access<KernelOutputTranslaterAddedEvent>(
                Guid.Parse("70f5bc18-3536-4306-9af7-256f323c9313"),
                "添加了内核输出翻译器后刷新VM内存",
                LogEnum.None,
                action: message => {
                    KernelOutputViewModel kernelOutputVm;
                    if (KernelOutputViewModels.Current.TryGetKernelOutputVm(message.Source.KernelOutputId, out kernelOutputVm)) {
                        if (!_dicByKernelOutputId.ContainsKey(message.Source.KernelOutputId)) {
                            _dicByKernelOutputId.Add(message.Source.KernelOutputId, new List<KernelOutputTranslaterViewModel>());
                        }
                        var vm = new KernelOutputTranslaterViewModel(message.Source);
                        _dicByKernelOutputId[message.Source.KernelOutputId].Add(vm);
                        _dicById.Add(message.Source.GetId(), vm);
                        kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputTranslaters));
                    }
                });
            Global.Access<KernelOutputTranslaterUpdatedEvent>(
                Guid.Parse("eef26e4b-af61-436b-9f24-9e128d614598"),
                "更新了内核输出翻译器后刷新VM内存",
                LogEnum.None,
                action: message => {
                    if (_dicByKernelOutputId.ContainsKey(message.Source.KernelOutputId)) {
                        var item = _dicByKernelOutputId[message.Source.KernelOutputId].FirstOrDefault(a => a.Id == message.Source.GetId());
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
                    if (_dicByKernelOutputId.ContainsKey(message.Source.KernelOutputId)) {
                        var item = _dicByKernelOutputId[message.Source.KernelOutputId].FirstOrDefault(a => a.Id == message.Source.GetId());
                        if (item != null) {
                            _dicByKernelOutputId[message.Source.KernelOutputId].Remove(item);
                        }
                    }
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        _dicById.Remove(message.Source.GetId());
                    }
                    KernelOutputViewModel kernelOutputVm;
                    if (KernelOutputViewModels.Current.TryGetKernelOutputVm(message.Source.KernelOutputId, out kernelOutputVm)) {
                        kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputTranslaters));
                    }
                });
            Init();
        }

        private void Init(bool isRefresh = false) {
            if (isRefresh) {
                foreach (var item in NTMinerRoot.Current.KernelOutputTranslaterSet) {
                    KernelOutputTranslaterViewModel vm;
                    if (_dicById.TryGetValue(item.GetId(), out vm)) {
                        Global.Execute(new UpdateKernelOutputTranslaterCommand(item));
                    }
                    else {
                        Global.Execute(new AddKernelOutputTranslaterCommand(item));
                    }
                }
            }
            else {
                foreach (var item in NTMinerRoot.Current.KernelOutputTranslaterSet) {
                    if (!_dicByKernelOutputId.ContainsKey(item.KernelOutputId)) {
                        _dicByKernelOutputId.Add(item.KernelOutputId, new List<KernelOutputTranslaterViewModel>());
                    }
                    var vm = new KernelOutputTranslaterViewModel(item);
                    _dicByKernelOutputId[item.KernelOutputId].Add(vm);
                    _dicById.Add(item.GetId(), vm);
                }
            }
        }

        public IEnumerable<KernelOutputTranslaterViewModel> AllKernelOutputTranslaterVms {
            get {
                return _dicById.Values;
            }
        }

        public IEnumerable<KernelOutputTranslaterViewModel> GetListByKernelId(Guid kernelId) {
            if (_dicByKernelOutputId.ContainsKey(kernelId)) {
                return _dicByKernelOutputId[kernelId];
            }
            return new List<KernelOutputTranslaterViewModel>();
        }
    }
}
