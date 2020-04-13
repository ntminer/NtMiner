using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class PoolKernelViewModels : ViewModelBase {
            public static readonly PoolKernelViewModels Instance = new PoolKernelViewModels();

            private readonly Dictionary<Guid, PoolKernelViewModel> _dicById = new Dictionary<Guid, PoolKernelViewModel>();
            private PoolKernelViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
#if DEBUG
                NTStopwatch.Start();
#endif
                AddEventPath<PoolKernelAddedEvent>("新添了矿池内核后刷新矿池内核VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (!_dicById.ContainsKey(message.Target.GetId())) {
                            if (PoolVms.TryGetPoolVm(message.Target.PoolId, out PoolViewModel poolVm)) {
                                _dicById.Add(message.Target.GetId(), new PoolKernelViewModel(message.Target));
                                poolVm.OnPropertyChanged(nameof(poolVm.PoolKernels));
                            }
                        }
                    }, location: this.GetType());
                AddEventPath<PoolKernelRemovedEvent>("移除了币种内核后刷新矿池内核VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.ContainsKey(message.Target.GetId())) {
                            var vm = _dicById[message.Target.GetId()];
                            _dicById.Remove(message.Target.GetId());
                            if (PoolVms.TryGetPoolVm(vm.PoolId, out PoolViewModel poolVm)) {
                                poolVm.OnPropertyChanged(nameof(poolVm.PoolKernels));
                            }
                        }
                    }, location: this.GetType());
                AddEventPath<PoolKernelUpdatedEvent>("更新了矿池内核后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.TryGetValue(message.Target.GetId(), out PoolKernelViewModel vm)) {
                            vm.Update(message.Target);
                        }
                    }, location: this.GetType());
                Init();
#if DEBUG
                var elapsedMilliseconds = NTStopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init() {
                foreach (IPoolKernel item in NTMinerContext.Instance.ServerContext.PoolKernelSet.AsEnumerable()) {
                    _dicById.Add(item.GetId(), new PoolKernelViewModel(item));
                }
            }

            public List<PoolKernelViewModel> AllPoolKernels {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}
