using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class PoolViewModels : ViewModelBase {
            public static PoolViewModels Instance { get; private set; } = new PoolViewModels();
            private readonly Dictionary<Guid, PoolViewModel> _dicById = new Dictionary<Guid, PoolViewModel>();
            private PoolViewModels() {
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
                        OnPropertyChanged(nameof(AllPools));
                    }, location: this.GetType());
                BuildEventPath<PoolAddedEvent>("添加矿池后刷新VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        _dicById.Add(message.Source.GetId(), new PoolViewModel(message.Source));
                        OnPropertyChanged(nameof(AllPools));
                        if (CoinVms.TryGetCoinVm(message.Source.CoinId, out CoinViewModel coinVm)) {
                            coinVm.CoinProfile?.OnPropertyChanged(nameof(CoinProfileViewModel.MainCoinPool));
                            coinVm.CoinProfile?.OnPropertyChanged(nameof(CoinProfileViewModel.DualCoinPool));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.Pools));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.OptionPools));
                        }
                    }, location: this.GetType());
                BuildEventPath<PoolRemovedEvent>("删除矿池后刷新VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(AllPools));
                        if (CoinVms.TryGetCoinVm(message.Source.CoinId, out CoinViewModel coinVm)) {
                            coinVm.CoinProfile?.OnPropertyChanged(nameof(CoinProfileViewModel.MainCoinPool));
                            coinVm.CoinProfile?.OnPropertyChanged(nameof(CoinProfileViewModel.DualCoinPool));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.Pools));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.OptionPools));
                        }
                    }, location: this.GetType());
                BuildEventPath<PoolUpdatedEvent>("更新矿池后刷新VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out PoolViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    }, location: this.GetType());
                Init();
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
