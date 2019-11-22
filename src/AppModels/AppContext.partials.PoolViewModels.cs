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
                Write.Stopwatch.Start();
#endif
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    });
                VirtualRoot.BuildEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChanged(nameof(AllPools));
                    });
                BuildEventPath<PoolAddedEvent>("添加矿池后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Add(message.Source.GetId(), new PoolViewModel(message.Source));
                        OnPropertyChanged(nameof(AllPools));
                        if (AppContext.Instance.CoinVms.TryGetCoinVm((Guid)message.Source.CoinId, out CoinViewModel coinVm)) {
                            coinVm.CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.MainCoinPool));
                            coinVm.CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.DualCoinPool));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.Pools));
                            coinVm.OnPropertyChanged(nameof(NTMiner.Vms.CoinViewModel.OptionPools));
                        }
                    });
                BuildEventPath<PoolRemovedEvent>("删除矿池后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(AllPools));
                        if (AppContext.Instance.CoinVms.TryGetCoinVm(message.Source.CoinId, out CoinViewModel coinVm)) {
                            coinVm.CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.MainCoinPool));
                            coinVm.CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.DualCoinPool));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.Pools));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.OptionPools));
                        }
                    });
                BuildEventPath<PoolUpdatedEvent>("更新矿池后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById[message.Source.GetId()].Update(message.Source);
                    });
                Init();
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > 20) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.ServerContext.PoolSet.AsEnumerable()) {
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

            public PoolViewModel GetNextOne(Guid coinId, int sortNumber) {
                return AllPools.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.CoinId == coinId && a.SortNumber > sortNumber);
            }

            public PoolViewModel GetUpOne(Guid coinId, int sortNumber) {
                return AllPools.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.CoinId == coinId && a.SortNumber < sortNumber);
            }
        }
    }
}
