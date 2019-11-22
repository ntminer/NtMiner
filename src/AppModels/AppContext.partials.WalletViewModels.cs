using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class WalletViewModels : ViewModelBase {
            public static readonly WalletViewModels Instance = new WalletViewModels();
            private readonly Dictionary<Guid, WalletViewModel> _dicById = new Dictionary<Guid, WalletViewModel>();
            private WalletViewModels() {
#if DEBUG
                Write.Stopwatch.Start();
#endif
                VirtualRoot.BuildEventPath<LocalContextReInitedEvent>("LocalContext刷新后刷新钱包Vm内存", LogEnum.None,
                    action: message=> {
                        _dicById.Clear();
                        Init();
                    });
                BuildEventPath<WalletAddedEvent>("添加了钱包后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Add(message.Source.GetId(), new WalletViewModel(message.Source));
                        OnPropertyChanged(nameof(WalletList));
                        CoinViewModel coin;
                        if (AppContext.Instance.CoinVms.TryGetCoinVm((Guid)message.Source.CoinId, out coin)) {
                            coin.OnPropertyChanged(nameof(CoinViewModel.Wallets));
                            coin.OnPropertyChanged(nameof(CoinViewModel.WalletItems));
                            coin.CoinKernel?.CoinKernelProfile?.SelectedDualCoin?.OnPropertyChanged(nameof(CoinViewModel.Wallets));
                        }
                    });
                BuildEventPath<WalletRemovedEvent>("删除了钱包后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(WalletList));
                        CoinViewModel coin;
                        if (AppContext.Instance.CoinVms.TryGetCoinVm((Guid)message.Source.CoinId, out coin)) {
                            coin.OnPropertyChanged(nameof(CoinViewModel.Wallets));
                            coin.OnPropertyChanged(nameof(CoinViewModel.WalletItems));
                            coin.CoinProfile?.OnPropertyChanged(nameof(CoinProfileViewModel.SelectedWallet));
                            coin.CoinKernel?.CoinKernelProfile?.SelectedDualCoin?.OnPropertyChanged(nameof(CoinViewModel.Wallets));
                        }
                    });
                BuildEventPath<WalletUpdatedEvent>("更新了钱包后调整VM内存", LogEnum.DevConsole,
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
                foreach (var item in NTMinerRoot.Instance.MinerProfile.GetWallets()) {
                    _dicById.Add(item.GetId(), new WalletViewModel(item));
                }
            }

            public List<WalletViewModel> WalletList {
                get {
                    return _dicById.Values.ToList();
                }
            }

            public WalletViewModel GetUpOne(Guid coinId, int sortNumber) {
                return WalletList.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.CoinId == coinId && a.SortNumber < sortNumber);
            }

            public WalletViewModel GetNextOne(Guid coinId, int sortNumber) {
                return WalletList.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.CoinId == coinId && a.SortNumber > sortNumber);
            }
        }
    }
}
