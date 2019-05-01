using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class KernelOutputTranslaterViewModels : ViewModelBase {
            private readonly Dictionary<Guid, List<KernelOutputTranslaterViewModel>> _dicByKernelOutputId = new Dictionary<Guid, List<KernelOutputTranslaterViewModel>>();
            private readonly Dictionary<Guid, KernelOutputTranslaterViewModel> _dicById = new Dictionary<Guid, KernelOutputTranslaterViewModel>();

            public KernelOutputTranslaterViewModels() {
                NTMinerRoot.Instance.OnContextReInited += () => {
                    _dicById.Clear();
                    _dicByKernelOutputId.Clear();
                    Init();
                };
                NTMinerRoot.Instance.OnReRendContext += () => {
                    OnPropertyChanged(nameof(AllKernelOutputTranslaterVms));
                };
                On<KernelOutputTranslaterAddedEvent>("添加了内核输出翻译器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        KernelOutputViewModel kernelOutputVm;
                        if (Current.KernelOutputVms.TryGetKernelOutputVm(message.Source.KernelOutputId, out kernelOutputVm)) {
                            if (!_dicByKernelOutputId.ContainsKey(message.Source.KernelOutputId)) {
                                _dicByKernelOutputId.Add(message.Source.KernelOutputId, new List<KernelOutputTranslaterViewModel>());
                            }
                            var vm = new KernelOutputTranslaterViewModel(message.Source);
                            _dicByKernelOutputId[message.Source.KernelOutputId].Add(vm);
                            _dicById.Add(message.Source.GetId(), vm);
                            kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputTranslaters));
                        }
                    });
                On<KernelOutputTranslaterUpdatedEvent>("更新了内核输出翻译器后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicByKernelOutputId.ContainsKey(message.Source.KernelOutputId)) {
                            var item = _dicByKernelOutputId[message.Source.KernelOutputId].FirstOrDefault(a => a.Id == message.Source.GetId());
                            if (item != null) {
                                item.Update(message.Source);
                            }
                        }
                    });
                On<KernelOutputTranslaterRemovedEvent>("移除了内核输出翻译器后刷新VM内存", LogEnum.DevConsole,
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
                        if (Current.KernelOutputVms.TryGetKernelOutputVm(message.Source.KernelOutputId, out kernelOutputVm)) {
                            kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputTranslaters));
                        }
                    });
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.KernelOutputTranslaterSet) {
                    if (!_dicByKernelOutputId.ContainsKey(item.KernelOutputId)) {
                        _dicByKernelOutputId.Add(item.KernelOutputId, new List<KernelOutputTranslaterViewModel>());
                    }
                    var vm = new KernelOutputTranslaterViewModel(item);
                    _dicByKernelOutputId[item.KernelOutputId].Add(vm);
                    _dicById.Add(item.GetId(), vm);
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
}
