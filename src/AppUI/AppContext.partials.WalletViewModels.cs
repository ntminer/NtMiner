using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class WalletViewModels : ViewModelBase {
            private readonly Dictionary<Guid, WalletViewModel> _dicById = new Dictionary<Guid, WalletViewModel>();
            public WalletViewModels() {
                VirtualRoot.On<WalletAddedEvent>("添加了钱包后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Add(message.Source.GetId(), new WalletViewModel(message.Source));
                        OnPropertyChanged(nameof(WalletList));
                        CoinViewModel coin;
                        if (AppContext.Current.CoinVms.TryGetCoinVm(message.Source.CoinId, out coin)) {
                            coin.OnPropertyChanged(nameof(CoinViewModel.Wallets));
                            coin.OnPropertyChanged(nameof(CoinViewModel.WalletItems));
                            coin.CoinKernel?.CoinKernelProfile?.SelectedDualCoin?.OnPropertyChanged(nameof(CoinViewModel.Wallets));
                        }
                    });
                VirtualRoot.On<WalletRemovedEvent>("删除了钱包后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(WalletList));
                        CoinViewModel coin;
                        if (AppContext.Current.CoinVms.TryGetCoinVm(message.Source.CoinId, out coin)) {
                            coin.OnPropertyChanged(nameof(CoinViewModel.Wallets));
                            coin.OnPropertyChanged(nameof(CoinViewModel.WalletItems));
                            coin.CoinProfile?.OnPropertyChanged(nameof(CoinProfileViewModel.SelectedWallet));
                            coin.CoinKernel?.CoinKernelProfile?.SelectedDualCoin?.OnPropertyChanged(nameof(CoinViewModel.Wallets));
                        }
                    });
                VirtualRoot.On<WalletUpdatedEvent>("更新了钱包后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById[message.Source.GetId()].Update(message.Source);
                    });
                foreach (var item in NTMinerRoot.Current.MinerProfile.GetWallets()) {
                    _dicById.Add(item.GetId(), new WalletViewModel(item));
                }
            }

            public List<WalletViewModel> WalletList {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}
