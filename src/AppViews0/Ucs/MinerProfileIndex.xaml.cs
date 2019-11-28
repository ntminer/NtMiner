using NTMiner.Vms;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NTMiner.Views.Ucs {
    public partial class MinerProfileIndex : UserControl {
        private MinerProfileIndexViewModel Vm {
            get {
                return (MinerProfileIndexViewModel)this.DataContext;
            }
        }

        public MinerProfileIndex() {
#if DEBUG
            Write.Stopwatch.Start();
#endif
            InitializeComponent();
            this.PopupKernel.Closed += Popup_Closed;
            this.PopupMainCoinPool.Closed += Popup_Closed;
            this.PopupMainCoin.Closed += Popup_Closed;
            this.PopupMainCoinWallet.Closed += Popup_Closed;
            this.RunOneceOnLoaded((window)=> {
                window.AddEventPath<ServerContextVmsReInitedEvent>("上下文视图模型集刷新后刷新界面上的popup", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        if (Vm.MinerProfile.MineWork != null) {
                            return;
                        }
                        if (this.PopupKernel.Child != null && this.PopupKernel.IsOpen) {
                            this.PopupKernel.IsOpen = false;
                            OpenKernelPopup();
                        }
                        if (this.PopupMainCoinPool.Child != null && this.PopupMainCoinPool.IsOpen) {
                            this.PopupMainCoinPool.IsOpen = false;
                            OpenMainCoinPoolPopup();
                        }
                        if (this.PopupMainCoinPool1.Child != null && this.PopupMainCoinPool1.IsOpen) {
                            this.PopupMainCoinPool1.IsOpen = false;
                            OpenMainCoinPool1Popup();
                        }
                        if (this.PopupMainCoin != null && this.PopupMainCoin.IsOpen) {
                            this.PopupMainCoin.IsOpen = false;
                            OpenMainCoinPopup();
                        }
                        if (this.PopupMainCoinWallet != null && this.PopupMainCoinWallet.IsOpen) {
                            this.PopupMainCoinWallet.IsOpen = false;
                            OpenMainCoinWalletPopup();
                        }
                    });
                }, location: this.GetType());
            });
#if DEBUG
            var elapsedMilliseconds = Write.Stopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
            }
#endif
        }

        private void Popup_Closed(object sender, System.EventArgs e) {
            ((Popup)sender).Child = null;
        }

        private void DualCoinWeightSlider_LostFocus(object sender, RoutedEventArgs e) {
            if (Vm.MinerProfile.CoinVm == null
                || Vm.MinerProfile.CoinVm.CoinKernel == null
                || Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile == null) {
                return;
            }
            CoinKernelProfileViewModel coinKernelProfileVm = Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile;
            NTMinerRoot.Instance.MinerProfile.SetCoinKernelProfileProperty(coinKernelProfileVm.CoinKernelId, nameof(coinKernelProfileVm.DualCoinWeight), coinKernelProfileVm.DualCoinWeight);
            NTMinerRoot.RefreshArgsAssembly.Invoke();
        }

        #region OpenPopup

        private void OpenKernelPopup() {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null) {
                return;
            }
            var popup = PopupKernel;
            popup.IsOpen = true;
            var selected = coinVm.CoinKernel;
            popup.Child = new CoinKernelSelect(
                new CoinKernelSelectViewModel(coinVm, selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (coinVm.CoinKernel != selectedResult) {
                            coinVm.CoinKernel = selectedResult;
                        }
                        else {
                            selectedResult?.Kernel?.OnPropertyChanged(nameof(selectedResult.Kernel.FullName));
                        }
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        private void OpenMainCoinPoolPopup() {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null) {
                return;
            }
            var popup = PopupMainCoinPool;
            popup.IsOpen = true;
            var selected = coinVm.CoinProfile.MainCoinPool;
            popup.Child = new PoolSelect(
                new PoolSelectViewModel(coinVm, selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (coinVm.CoinProfile.MainCoinPool != selectedResult) {
                            coinVm.CoinProfile.MainCoinPool = selectedResult;
                        }
                        else {
                            selectedResult.OnPropertyChanged(nameof(selectedResult.Name));
                        }
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        private void OpenMainCoinPool1Popup() {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null) {
                return;
            }
            var popup = PopupMainCoinPool1;
            popup.IsOpen = true;
            var selected = coinVm.CoinProfile.MainCoinPool1;
            popup.Child = new PoolSelect(
                new PoolSelectViewModel(coinVm, selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (coinVm.CoinProfile.MainCoinPool1 != selectedResult) {
                            coinVm.CoinProfile.MainCoinPool1 = selectedResult;
                        }
                        else {
                            selectedResult.OnPropertyChanged(nameof(selectedResult.Name));
                        }
                        popup.IsOpen = false;
                    }
                }, usedByPool1: true) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        private void OpenMainCoinPopup() {
            var popup = PopupMainCoin;
            popup.IsOpen = true;
            var selected = Vm.MinerProfile.CoinVm;
            popup.Child = new CoinSelect(
                new CoinSelectViewModel(AppContext.Instance.CoinVms.MainCoins.Where(a => a.IsSupported), selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (Vm.MinerProfile.CoinVm != selectedResult) {
                            Vm.MinerProfile.CoinVm = selectedResult;
                        }
                        else {
                            selectedResult.OnPropertyChanged(nameof(selectedResult.Code));
                        }
                        popup.IsOpen = false;
                    }
                }, isPromoteHotCoin: true) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        private void OpenMainCoinWalletPopup() {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null) {
                return;
            }
            var popup = PopupMainCoinWallet;
            popup.IsOpen = true;
            var selected = coinVm.CoinProfile.SelectedWallet;
            bool isDualCoin = false;
            popup.Child = new WalletSelect(
                new WalletSelectViewModel(coinVm, isDualCoin, selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (coinVm.CoinProfile.SelectedWallet != selectedResult) {
                            coinVm.CoinProfile.SelectedWallet = selectedResult;
                        }
                        else {
                            coinVm.CoinProfile.OnPropertyChanged(nameof(coinVm.CoinProfile.SelectedWallet));
                            selectedResult.OnPropertyChanged(nameof(selectedResult.Name));
                            selectedResult.OnPropertyChanged(nameof(selectedResult.Address));
                        }
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        #endregion

        private void KbButtonKernel_Clicked(object sender, RoutedEventArgs e) {
            OpenKernelPopup();
            UserActionHappend();
            e.Handled = true;
        }

        private void KbButtonMainCoinPool_Clicked(object sender, RoutedEventArgs e) {
            OpenMainCoinPoolPopup();
            UserActionHappend();
            e.Handled = true;
        }

        private void KbButtonMainCoin_Clicked(object sender, RoutedEventArgs e) {
            OpenMainCoinPopup();
            UserActionHappend();
            e.Handled = true;
        }

        private void KbButtonMainCoinWallet_Clicked(object sender, RoutedEventArgs e) {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null) {
                return;
            }
            if (coinVm.Wallets.Count == 0) {
                coinVm.CoinProfile.AddWallet.Execute(null);
            }
            else {
                OpenMainCoinWalletPopup();
                UserActionHappend();
            }
            e.Handled = true;
        }

        private static void UserActionHappend() {
            if (!DevMode.IsDebugMode) {
                VirtualRoot.RaiseEvent(new UserActionEvent());
            }
        }

        private void DualContainer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (DualContainer.Visibility == Visibility.Visible && DualContainer.Child == null) {
                MinerProfileDual child = new MinerProfileDual();
                DualContainer.Child = child;
            }
        }

        private void KbButtonMainCoinPool1_Clicked(object sender, RoutedEventArgs e) {
            OpenMainCoinPool1Popup();
            UserActionHappend();
            e.Handled = true;
        }
    }
}
