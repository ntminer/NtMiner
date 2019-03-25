using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class KernelInputViewModels : ViewModelBase {
        public static readonly KernelInputViewModels Current = new KernelInputViewModels();

        private readonly Dictionary<Guid, KernelInputViewModel> _dicById = new Dictionary<Guid, KernelInputViewModel>();

        private KernelInputViewModels() {
            NTMinerRoot.Current.OnContextReInited += () => {
                _dicById.Clear();
                Init();
            };
            NTMinerRoot.Current.OnReRendContext += () => {
                AllPropertyChanged();
            };
            Init();
        }

        private void Init() {
            VirtualRoot.On<KernelInputAddedEvent>("添加了内核输入后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    var vm = new KernelInputViewModel(message.Source);
                    _dicById.Add(message.Source.GetId(), vm);
                    OnPropertyChangeds();
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            VirtualRoot.On<KernelInputUpdatedEvent>("更新了内核输入后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        var item = _dicById[message.Source.GetId()];
                        if (item != null) {
                            bool isSupportDualMine = item.IsSupportDualMine;
                            string args = item.Args;
                            string dualFullArgs = item.DualFullArgs;
                            item.Update(message.Source);
                            if (args != item.Args || dualFullArgs != item.DualFullArgs) {
                                CoinViewModel coinVm = MinerProfileViewModel.Current.CoinVm;
                                if (coinVm != null && coinVm.CoinKernel != null && coinVm.CoinKernel.Kernel.KernelInputId == item.Id) {
                                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                                }
                            }
                            if (isSupportDualMine != item.IsSupportDualMine) {
                                foreach (var coinKernelVm in CoinKernelViewModels.Current.AllCoinKernels.Where(a => a.KernelId == message.Source.GetId())) {
                                    coinKernelVm.OnPropertyChanged(nameof(coinKernelVm.IsSupportDualMine));
                                    coinKernelVm.OnPropertyChanged(nameof(coinKernelVm.DualCoinGroup));
                                }
                            }
                        }
                    }
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            VirtualRoot.On<KernelInputRemovedEvent>("移除了内核输入后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChangeds();
                    }
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            foreach (var item in NTMinerRoot.Current.KernelInputSet) {
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
