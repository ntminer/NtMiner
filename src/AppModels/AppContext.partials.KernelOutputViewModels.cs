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
                Write.Stopwatch.Start();
#endif
                VirtualRoot.AddEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                VirtualRoot.AddEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        AllPropertyChanged();
                    }, location: this.GetType());
                AddEventPath<KernelOutputAddedEvent>("添加了内核输出组后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        var vm = new KernelOutputViewModel(message.Target);
                        _dicById.Add(message.Target.GetId(), vm);
                        OnPropertyChanged(nameof(AllKernelOutputVms));
                        OnPropertyChanged(nameof(PleaseSelectVms));
                    }, location: this.GetType());
                AddEventPath<KernelOutputUpdatedEvent>("更新了内核输出组后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.ContainsKey(message.Target.GetId())) {
                            var item = _dicById[message.Target.GetId()];
                            if (item != null) {
                                item.Update(message.Target);
                            }
                        }
                    }, location: this.GetType());
                AddEventPath<KernelOutputRemovedEvent>("移除了内核输出组后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.ContainsKey(message.Target.GetId())) {
                            _dicById.Remove(message.Target.GetId());
                            OnPropertyChanged(nameof(AllKernelOutputVms));
                            OnPropertyChanged(nameof(PleaseSelectVms));
                        }
                    }, location: this.GetType());
                Init();
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.ServerContext.KernelOutputSet.AsEnumerable()) {
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
