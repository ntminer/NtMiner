using NTMiner.Core;
using NTMiner.Core.Kernels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class PoolKernelViewModels : ViewModelBase {
        public static readonly PoolKernelViewModels Current = new PoolKernelViewModels();
        private readonly Dictionary<Guid, PoolKernelViewModel> _dicById = new Dictionary<Guid, PoolKernelViewModel>();
        private PoolKernelViewModels() {
            Global.Access<PoolKernelSetRefreshedEvent>(
                Guid.Parse("2F8E3D90-F2E3-43FF-A962-5E5A73AA5CE4"),
                "矿池内核数据集刷新后刷新Vm内存",
                LogEnum.Console,
                action: message => {
                    Init(isRefresh: true);
                });
            Global.Access<PoolKernelAddedEvent>(
                Guid.Parse("75C01641-A50D-4880-826F-83F56C817B82"),
                "新添了矿池内核后刷新矿池内核VM内存",
                LogEnum.Console,
                action: (message) => {
                    if (!_dicById.ContainsKey(message.Source.GetId())) {
                        PoolViewModel poolVm;
                        if (PoolViewModels.Current.TryGetPoolVm(message.Source.PoolId, out poolVm)) {
                            _dicById.Add(message.Source.GetId(), new PoolKernelViewModel(message.Source));
                            poolVm.OnPropertyChanged(nameof(poolVm.PoolKernels));
                        }
                    }
                });
            Global.Access<PoolKernelRemovedEvent>(
                Guid.Parse("777F7A90-CCBA-44C7-877C-471E65828A0B"),
                "移除了币种内核后刷新矿池内核VM内存",
                LogEnum.Console,
                action: (message) => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        var vm = _dicById[message.Source.GetId()];
                        _dicById.Remove(message.Source.GetId());
                        PoolViewModel poolVm;
                        if (PoolViewModels.Current.TryGetPoolVm(vm.PoolId, out poolVm)) {
                            poolVm.OnPropertyChanged(nameof(poolVm.PoolKernels));
                        }
                    }
                });
            Global.Access<PoolKernelUpdatedEvent>(
                Guid.Parse("A5A1F722-735E-4792-BAFC-F050CC4909BB"),
                "更新了矿池内核后刷新VM内存",
                LogEnum.Console,
                action: (message) => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        _dicById[message.Source.GetId()].Update(message.Source);
                    }
                });
            Init();
        }

        private void Init(bool isRefresh = false) {
            if (isRefresh) {
                foreach (IPoolKernel item in NTMinerRoot.Current.PoolKernelSet) {
                    PoolKernelViewModel vm;
                    if (_dicById.TryGetValue(item.GetId(), out vm)) {
                        Global.Execute(new UpdatePoolKernelCommand(item));
                    }
                    else {
                        Global.Execute(new AddPoolKernelCommand(item));
                    }
                }
            }
            else {
                foreach (IPoolKernel item in NTMinerRoot.Current.PoolKernelSet) {
                    _dicById.Add(item.GetId(), new PoolKernelViewModel(item));
                }
            }
        }

        public List<PoolKernelViewModel> AllPoolKernels {
            get {
                return _dicById.Values.ToList();
            }
        }
    }
}
