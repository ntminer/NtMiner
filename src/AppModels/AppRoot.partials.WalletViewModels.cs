using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class WalletViewModels : ViewModelBase {
            public static WalletViewModels Instance { get; private set; } = new WalletViewModels();
            private readonly Dictionary<Guid, WalletViewModel> _dicById = new Dictionary<Guid, WalletViewModel>();
            private WalletViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                VirtualRoot.BuildEventPath<LocalContextReInitedEvent>("刷新钱包Vm内存", LogEnum.None,
                    path: message=> {
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                BuildEventPath<WalletAddedEvent>("调整VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        _dicById.Add(message.Source.GetId(), new WalletViewModel(message.Source));
                        OnPropertyChanged(nameof(WalletList));
                        if (CoinVms.TryGetCoinVm(message.Source.CoinId, out CoinViewModel coin)) {
                            coin.OnPropertyChanged(nameof(CoinViewModel.Wallets));
                            coin.OnPropertyChanged(nameof(CoinViewModel.WalletItems));
                            coin.CoinKernel?.CoinKernelProfile?.SelectedDualCoin?.OnPropertyChanged(nameof(CoinViewModel.Wallets));
                        }
                        VirtualRoot.RaiseEvent(new WalletVmAddedEvent(message));
                    }, location: this.GetType());
                BuildEventPath<WalletRemovedEvent>("调整VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(WalletList));
                        if (CoinVms.TryGetCoinVm(message.Source.CoinId, out CoinViewModel coin)) {
                            coin.OnPropertyChanged(nameof(CoinViewModel.Wallets));
                            coin.OnPropertyChanged(nameof(CoinViewModel.WalletItems));
                            coin.CoinProfile?.OnPropertyChanged(nameof(CoinProfileViewModel.SelectedWallet));
                            coin.CoinKernel?.CoinKernelProfile?.SelectedDualCoin?.OnPropertyChanged(nameof(CoinViewModel.Wallets));
                        }
                        VirtualRoot.RaiseEvent(new WalletVmRemovedEvent(message));
                    }, location: this.GetType());
                BuildEventPath<WalletUpdatedEvent>("调整VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out WalletViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    }, location: this.GetType());
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.MinerProfile.GetWallets()) {
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
