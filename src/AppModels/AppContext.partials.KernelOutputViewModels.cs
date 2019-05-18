using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class KernelOutputViewModels : ViewModelBase {
            public static readonly KernelOutputViewModels Instance = new KernelOutputViewModels();
            private readonly Dictionary<Guid, KernelOutputViewModel> _dicById = new Dictionary<Guid, KernelOutputViewModel>();

            private KernelOutputViewModels() {
#if DEBUG
                VirtualRoot.Stopwatch.Restart();
#endif
                VirtualRoot.On<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    });
                VirtualRoot.On<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        AllPropertyChanged();
                    });
                On<KernelOutputAddedEvent>("添加了内核输出组后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        var vm = new KernelOutputViewModel(message.Source);
                        _dicById.Add(message.Source.GetId(), vm);
                        OnPropertyChanged(nameof(AllKernelOutputVms));
                        OnPropertyChanged(nameof(PleaseSelectVms));
                    });
                On<KernelOutputUpdatedEvent>("更新了内核输出组后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            var item = _dicById[message.Source.GetId()];
                            if (item != null) {
                                item.Update(message.Source);
                            }
                        }
                    });
                On<KernelOutputRemovedEvent>("移除了内核输出组后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Remove(message.Source.GetId());
                            OnPropertyChanged(nameof(AllKernelOutputVms));
                            OnPropertyChanged(nameof(PleaseSelectVms));
                        }
                    });
                Init();
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.KernelOutputSet) {
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
