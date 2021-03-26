using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class KernelOutputViewModels : ViewModelBase {
            public static KernelOutputViewModels Instance { get; private set; } = new KernelOutputViewModels();
            private readonly Dictionary<Guid, KernelOutputViewModel> _dicById = new Dictionary<Guid, KernelOutputViewModel>();

            private KernelOutputViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                VirtualRoot.BuildEventPath<ServerContextReInitedEventHandledEvent>("刷新视图界面", LogEnum.DevConsole,
                    path: message => {
                        AllPropertyChanged();
                    }, location: this.GetType());
                BuildEventPath<KernelOutputAddedEvent>("刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        var vm = new KernelOutputViewModel(message.Source);
                        _dicById.Add(message.Source.GetId(), vm);
                        OnPropertyChanged(nameof(AllKernelOutputVms));
                        OnPropertyChanged(nameof(PleaseSelectVms));
                    }, location: this.GetType());
                BuildEventPath<KernelOutputUpdatedEvent>("刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out KernelOutputViewModel vm)) {
                            if (vm != null) {
                                vm.Update(message.Source);
                            }
                        }
                    }, location: this.GetType());
                BuildEventPath<KernelOutputRemovedEvent>("刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Remove(message.Source.GetId());
                            OnPropertyChanged(nameof(AllKernelOutputVms));
                            OnPropertyChanged(nameof(PleaseSelectVms));
                        }
                    }, location: this.GetType());
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.ServerContext.KernelOutputSet.AsEnumerable().ToArray()) {
                    _dicById.Add(item.GetId(), new KernelOutputViewModel(item));
                }
            }

            public bool TryGetKernelOutputVm(Guid id, out KernelOutputViewModel kernelOutputVm) {
                return _dicById.TryGetValue(id, out kernelOutputVm);
            }

            public List<KernelOutputViewModel> AllKernelOutputVms {
                get {
                    return _dicById.Values.OrderBy(a => a.Name).ToList();
                }
            }

            private IEnumerable<KernelOutputViewModel> GetPleaseSelectVms() {
                yield return KernelOutputViewModel.PleaseSelect;
                foreach (var item in _dicById.Values.OrderBy(a => a.Name)) {
                    yield return item;
                }
            }

            public List<KernelOutputViewModel> PleaseSelectVms {
                get {
                    return GetPleaseSelectVms().ToList();
                }
            }
        }
    }
}
