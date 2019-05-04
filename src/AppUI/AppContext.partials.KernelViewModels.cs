using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class KernelViewModels : ViewModelBase {
            public static readonly KernelViewModels Instance = new KernelViewModels();
            private readonly Dictionary<Guid, KernelViewModel> _dicById = new Dictionary<Guid, KernelViewModel>();
            public event Action<KernelViewModel> IsDownloadingChanged;

            public void OnIsDownloadingChanged(KernelViewModel kernelVm) {
                IsDownloadingChanged?.Invoke(kernelVm);
            }

            private KernelViewModels() {
                NTMinerRoot.Instance.OnContextReInited += () => {
                    _dicById.Clear();
                    Init();
                };
                NTMinerRoot.Instance.OnReRendContext += () => {
                    OnPropertyChanged(nameof(AllKernels));
                };
                On<KernelAddedEvent>("添加了内核后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Add(message.Source.GetId(), new KernelViewModel(message.Source));
                        OnPropertyChanged(nameof(AllKernels));
                        foreach (var coinKernelVm in Current.CoinKernelVms.AllCoinKernels.Where(a => a.KernelId == message.Source.GetId())) {
                            coinKernelVm.OnPropertyChanged(nameof(coinKernelVm.IsSupportDualMine));
                        }
                    });
                On<KernelRemovedEvent>("删除了内核后调整VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(AllKernels));
                        foreach (var coinKernelVm in Current.CoinKernelVms.AllCoinKernels.Where(a => a.KernelId == message.Source.GetId())) {
                            coinKernelVm.OnPropertyChanged(nameof(coinKernelVm.IsSupportDualMine));
                        }
                    });
                On<KernelUpdatedEvent>("更新了内核后调整VM内存", LogEnum.DevConsole,
                    action: message => {
                        var entity = _dicById[message.Source.GetId()];
                        PublishStatus publishStatus = entity.PublishState;
                        Guid kernelInputId = entity.KernelInputId;
                        entity.Update(message.Source);
                        if (publishStatus != entity.PublishState) {
                            foreach (var coinKernelVm in Current.CoinKernelVms.AllCoinKernels.Where(a => a.KernelId == entity.Id)) {
                                foreach (var coinVm in Current.CoinVms.AllCoins.Where(a => a.Id == coinKernelVm.CoinId)) {
                                    coinVm.OnPropertyChanged(nameof(coinVm.CoinKernels));
                                }
                            }
                        }
                        if (kernelInputId != entity.KernelInputId) {
                            NTMinerRoot.RefreshArgsAssembly.Invoke();
                        }
                    });
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.KernelSet) {
                    _dicById.Add(item.GetId(), new KernelViewModel(item));
                }
            }

            public bool TryGetKernelVm(Guid kernelId, out KernelViewModel kernelVm) {
                return _dicById.TryGetValue(kernelId, out kernelVm);
            }

            public List<KernelViewModel> AllKernels {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}
