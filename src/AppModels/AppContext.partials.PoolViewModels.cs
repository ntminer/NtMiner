using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class PoolViewModels : ViewModelBase {
            public static readonly PoolViewModels Instance = new PoolViewModels();
            private readonly Dictionary<Guid, PoolViewModel> _dicById = new Dictionary<Guid, PoolViewModel>();
            private PoolViewModels() {
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
                        OnPropertyChanged(nameof(AllPools));
                    });
                On<PoolAddedEvent>("添加矿池后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Add(message.Source.GetId(), new PoolViewModel(message.Source));
                        OnPropertyChanged(nameof(AllPools));
                        CoinViewModel coinVm;
                        if (AppContext.Instance.CoinVms.TryGetCoinVm((Guid)message.Source.CoinId, out coinVm)) {
                            coinVm.CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.MainCoinPool));
                            coinVm.CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.DualCoinPool));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.Pools));
                            coinVm.OnPropertyChanged(nameof(NTMiner.Vms.CoinViewModel.OptionPools));
                        }
                    });
                On<PoolRemovedEvent>("删除矿池后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(AllPools));
                        CoinViewModel coinVm;
                        if (AppContext.Instance.CoinVms.TryGetCoinVm(message.Source.CoinId, out coinVm)) {
                            coinVm.CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.MainCoinPool));
                            coinVm.CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.DualCoinPool));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.Pools));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.OptionPools));
                        }
                    });
                On<PoolUpdatedEvent>("更新矿池后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById[message.Source.GetId()].Update(message.Source);
                    });
                Init();
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.PoolSet) {
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
