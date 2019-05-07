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
            InitializeComponent();
            this.PopupKernel.Closed += Popup_Closed;
            this.PopupMainCoinPool.Closed += Popup_Closed;
            this.PopupDualCoinPool.Closed += Popup_Closed;
            this.PopupMainCoin.Closed += Popup_Closed;
            this.PopupDualCoin.Closed += Popup_Closed;
            this.PopupMainCoinWallet.Closed += Popup_Closed;
            this.PopupDualCoinWallet.Closed += Popup_Closed;
            this.On<LocalContextVmsReInitedEvent>("本地上下文视图模型集刷新后刷新界面上的popup", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        if (Vm.MinerProfile.MineWork != null) {
                            return;
                        }
                        if (this.PopupKernel.Child != null) {
                            this.PopupKernel.IsOpen = false;
                            OpenKernelPopup();
                        }
                        if (this.PopupMainCoinPool.Child != null) {
                            this.PopupMainCoinPool.IsOpen = false;
                            OpenMainCoinPoolPopup();
                        }
                        if (this.PopupDualCoinPool != null) {
                            this.PopupDualCoinPool.IsOpen = false;
                            OpenDualCoinPoolPopup();
                        }
                        if (this.PopupMainCoin != null) {
                            this.PopupMainCoin.IsOpen = false;
                            OpenMainCoinPopup();
                        }
                        if (this.PopupDualCoin != null) {
                            this.PopupDualCoin.IsOpen = false;
                            OpenDualCoinPopup();
                        }
                        if (this.PopupMainCoinWallet != null) {
                            this.PopupMainCoinWallet.IsOpen = false;
                            OpenMainCoinWalletPopup();
                        }
                        if (this.PopupDualCoinWallet != null) {
                            this.PopupDualCoinWallet.IsOpen = false;
                            OpenDualCoinWalletPopup();
                        }
                    });
                });
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
                        if (selectedResult != coinVm.CoinKernel) {
                            coinVm.CoinKernel = selectedResult;
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
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        private void OpenDualCoinPoolPopup() {
            var coinVm = Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin;
            if (coinVm == null) {
                return;
            }
            var popup = PopupDualCoinPool;
            popup.IsOpen = true;
            var selected = coinVm.CoinProfile.DualCoinPool;
            popup.Child = new PoolSelect(
                new PoolSelectViewModel(coinVm, selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (coinVm.CoinProfile.DualCoinPool != selectedResult) {
                            coinVm.CoinProfile.DualCoinPool = selectedResult;
                        }
                        popup.IsOpen = false;
                    }
                }) {
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
                    if (selectedResult != null && selectedResult != Vm.MinerProfile.CoinVm) {
                        Vm.MinerProfile.CoinVm = selectedResult;
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        private void OpenDualCoinPopup() {
            if (Vm.MinerProfile.CoinVm == null || Vm.MinerProfile.CoinVm.CoinKernel == null) {
                return;
            }
            var popup = PopupDualCoin;
            popup.IsOpen = true;
            var selected = Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin;
            popup.Child = new CoinSelect(
                new CoinSelectViewModel(Vm.MinerProfile.CoinVm.CoinKernel.DualCoinGroup.DualCoinVms, selected, onOk: selectedResult => {
                    if (selectedResult != null && Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin != selectedResult) {
                        Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin = selectedResult;
                        popup.IsOpen = false;
                    }
                }) {
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
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        private void OpenDualCoinWalletPopup() {
            var coinVm = Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin;
            if (coinVm == null) {
                return;
            }
            var popup = PopupDualCoinWallet;
            popup.IsOpen = true;
            var selected = coinVm.CoinProfile.SelectedDualCoinWallet;
            bool isDualCoin = true;
            popup.Child = new WalletSelect(
                new WalletSelectViewModel(coinVm, isDualCoin, selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (coinVm.CoinProfile.SelectedDualCoinWallet != selectedResult) {
                            coinVm.CoinProfile.SelectedDualCoinWallet = selectedResult;
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
        }

        private void KbButtonMainCoinPool_Clicked(object sender, RoutedEventArgs e) {
            OpenMainCoinPoolPopup();
            UserActionHappend();
        }

        private void KbButtonDualCoinPool_Clicked(object sender, RoutedEventArgs e) {
            OpenDualCoinPoolPopup();
            UserActionHappend();
        }

        private void KbButtonMainCoin_Clicked(object sender, RoutedEventArgs e) {
            OpenMainCoinPopup();
            UserActionHappend();
        }

        private void KbButtonDualCoin_Clicked(object sender, RoutedEventArgs e) {
            OpenDualCoinPopup();
            UserActionHappend();
        }

        private void KbButtonMainCoinWallet_Clicked(object sender, RoutedEventArgs e) {
            OpenMainCoinWalletPopup();
            UserActionHappend();
        }

        private void KbButtonDualCoinWallet_Clicked(object sender, RoutedEventArgs e) {
            OpenDualCoinWalletPopup();
            UserActionHappend();
        }

        private static void UserActionHappend() {
            if (!DevMode.IsDebugMode) {
                VirtualRoot.Happened(new UserActionEvent());
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
