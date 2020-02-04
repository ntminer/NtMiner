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
#if DEBUG
                NTStopwatch.Start();
#endif
                VirtualRoot.AddEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                VirtualRoot.AddEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChanged(nameof(AllKernels));
                    }, location: this.GetType());
                AddEventPath<KernelAddedEvent>("添加了内核后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Add(message.Target.GetId(), new KernelViewModel(message.Target));
                        OnPropertyChanged(nameof(AllKernels));
                        foreach (var coinKernelVm in AppContext.Instance.CoinKernelVms.AllCoinKernels.Where(a => a.KernelId == message.Target.GetId())) {
                            coinKernelVm.OnPropertyChanged(nameof(coinKernelVm.IsSupportDualMine));
                        }
                    }, location: this.GetType());
                AddEventPath<KernelRemovedEvent>("删除了内核后调整VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Remove(message.Target.GetId());
                        OnPropertyChanged(nameof(AllKernels));
                        foreach (var coinKernelVm in AppContext.Instance.CoinKernelVms.AllCoinKernels.Where(a => a.KernelId == message.Target.GetId())) {
                            coinKernelVm.OnPropertyChanged(nameof(coinKernelVm.IsSupportDualMine));
                        }
                    }, location: this.GetType());
                AddEventPath<KernelUpdatedEvent>("更新了内核后调整VM内存", LogEnum.DevConsole,
                    action: message => {
                        var entity = _dicById[message.Target.GetId()];
                        PublishStatus publishStatus = entity.PublishState;
                        Guid kernelInputId = entity.KernelInputId;
                        entity.Update(message.Target);
                        if (publishStatus != entity.PublishState) {
                            foreach (var coinKernelVm in AppContext.Instance.CoinKernelVms.AllCoinKernels.Where(a => a.KernelId == entity.Id)) {
                                foreach (var coinVm in AppContext.Instance.CoinVms.AllCoins.Where(a => a.Id == coinKernelVm.CoinId)) {
                                    coinVm.OnPropertyChanged(nameof(coinVm.CoinKernels));
                                }
                            }
                        }
                        if (kernelInputId != entity.KernelInputId) {
                            NTMinerRoot.RefreshArgsAssembly.Invoke();
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
                foreach (var item in NTMinerRoot.Instance.ServerContext.KernelSet.AsEnumerable()) {
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
