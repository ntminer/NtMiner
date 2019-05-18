using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class KernelInputViewModels : ViewModelBase {
            public static readonly KernelInputViewModels Instance = new KernelInputViewModels();
            private readonly Dictionary<Guid, KernelInputViewModel> _dicById = new Dictionary<Guid, KernelInputViewModel>();

            private KernelInputViewModels() {
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
                        OnPropertyChangeds();
                    });
                On<KernelInputAddedEvent>("添加了内核输入后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        var vm = new KernelInputViewModel(message.Source);
                        _dicById.Add(message.Source.GetId(), vm);
                        OnPropertyChangeds();
                    });
                On<KernelInputUpdatedEvent>("更新了内核输入后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            var item = _dicById[message.Source.GetId()];
                            if (item != null) {
                                bool isSupportDualMine = item.IsSupportDualMine;
                                string args = item.Args;
                                string dualFullArgs = item.DualFullArgs;
                                item.Update(message.Source);
                                if (args != item.Args || dualFullArgs != item.DualFullArgs) {
                                    CoinViewModel coinVm = AppContext.Instance.MinerProfileVm.CoinVm;
                                    if (coinVm != null && coinVm.CoinKernel != null && coinVm.CoinKernel.Kernel.KernelInputId == item.Id) {
                                        NTMinerRoot.RefreshArgsAssembly.Invoke();
                                    }
                                }
                                if (isSupportDualMine != item.IsSupportDualMine) {
                                    foreach (var coinKernelVm in AppContext.Instance.CoinKernelVms.AllCoinKernels.Where(a => a.KernelId == message.Source.GetId())) {
                                        coinKernelVm.OnPropertyChanged(nameof(coinKernelVm.IsSupportDualMine));
                                        coinKernelVm.OnPropertyChanged(nameof(coinKernelVm.DualCoinGroup));
                                    }
                                }
                            }
                        }
                    });
                On<KernelInputRemovedEvent>("移除了内核输入后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Remove(message.Source.GetId());
                            OnPropertyChangeds();
                        }
                    });
                Init();
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.KernelInputSet) {
                    _dicById.Add(item.GetId(), new KernelInputViewModel(item));
                }
            }

            private void OnPropertyChangeds() {
                OnPropertyChanged(nameof(AllKernelInputVms));
                OnPropertyChanged(nameof(PleaseSelectVms));
            }

            public bool TryGetKernelInputVm(Guid id, out KernelInputViewModel kernelInputVm) {
                return _dicById.TryGetValue(id, out kernelInputVm);
            }

            public List<KernelInputViewModel> AllKernelInputVms {
                get {
                    return _dicById.Values.OrderBy(a => a.Name).ToList();
                }
            }

            private IEnumerable<KernelInputViewModel> GetPleaseSelectVms() {
                yield return KernelInputViewModel.PleaseSelect;
                foreach (var item in _dicById.Values.OrderBy(a => a.Name)) {
                    yield return item;
                }
            }

            public List<KernelInputViewModel> PleaseSelectVms {
                get {
                    return GetPleaseSelectVms().ToList();
                }
            }
        }
    }
}
