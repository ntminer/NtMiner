using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class PoolViewModels : ViewModelBase {
            public static readonly PoolViewModels Instance = new PoolViewModels();
            private readonly Dictionary<Guid, PoolViewModel> _dicById = new Dictionary<Guid, PoolViewModel>();
            private PoolViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
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
                        OnPropertyChanged(nameof(AllPools));
                    }, location: this.GetType());
                AddEventPath<PoolAddedEvent>("添加矿池后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Add(message.Target.GetId(), new PoolViewModel(message.Target));
                        OnPropertyChanged(nameof(AllPools));
                        if (CoinVms.TryGetCoinVm(message.Target.CoinId, out CoinViewModel coinVm)) {
                            coinVm.CoinProfile?.OnPropertyChanged(nameof(CoinProfileViewModel.MainCoinPool));
                            coinVm.CoinProfile?.OnPropertyChanged(nameof(CoinProfileViewModel.DualCoinPool));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.Pools));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.OptionPools));
                        }
                    }, location: this.GetType());
                AddEventPath<PoolRemovedEvent>("删除矿池后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Target.GetId());
                        OnPropertyChanged(nameof(AllPools));
                        if (CoinVms.TryGetCoinVm(message.Target.CoinId, out CoinViewModel coinVm)) {
                            coinVm.CoinProfile?.OnPropertyChanged(nameof(CoinProfileViewModel.MainCoinPool));
                            coinVm.CoinProfile?.OnPropertyChanged(nameof(CoinProfileViewModel.DualCoinPool));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.Pools));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.OptionPools));
                        }
                    }, location: this.GetType());
                AddEventPath<PoolUpdatedEvent>("更新矿池后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.TryGetValue(message.Target.GetId(), out PoolViewModel vm)) {
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
                foreach (var item in NTMinerContext.Instance.ServerContext.PoolSet.AsEnumerable()) {
                    _dicById.Add(item.GetId(), new PoolViewModel(item));
                }
            }

            public bool TryGetPoolVm(Guid poolId, out PoolViewModel poolVm) {
                return _dicById.TryGetValue(poolId, out poolVm);
            }

            public List<PoolViewModel> AllPools {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}
