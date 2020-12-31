using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class PoolKernelViewModels : ViewModelBase {
            public static PoolKernelViewModels Instance { get; private set; } = new PoolKernelViewModels();

            private readonly Dictionary<Guid, PoolKernelViewModel> _dicById = new Dictionary<Guid, PoolKernelViewModel>();
            private PoolKernelViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                VirtualRoot.BuildEventPath<ServerContextReInitedEventHandledEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    path: message => {
                        OnPropertyChanged(nameof(AllPoolKernels));
                    }, location: this.GetType());
                BuildEventPath<PoolKernelAddedEvent>("新添了矿池内核后刷新矿池内核VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            if (PoolVms.TryGetPoolVm(message.Source.PoolId, out PoolViewModel poolVm)) {
                                _dicById.Add(message.Source.GetId(), new PoolKernelViewModel(message.Source));
                                poolVm.OnPropertyChanged(nameof(poolVm.PoolKernels));
                            }
                        }
                    }, location: this.GetType());
                BuildEventPath<PoolKernelRemovedEvent>("移除了币种内核后刷新矿池内核VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            var vm = _dicById[message.Source.GetId()];
                            _dicById.Remove(message.Source.GetId());
                            if (PoolVms.TryGetPoolVm(vm.PoolId, out PoolViewModel poolVm)) {
                                poolVm.OnPropertyChanged(nameof(poolVm.PoolKernels));
                            }
                        }
                    }, location: this.GetType());
                BuildEventPath<PoolKernelUpdatedEvent>("更新了矿池内核后刷新VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out PoolKernelViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    }, location: this.GetType());
                Init();
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
