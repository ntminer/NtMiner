using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NTMiner.Views.Ucs {
    public partial class MinerProfileDual : UserControl {
        private AppContext.MinerProfileViewModel Vm {
            get {
                return AppContext.MinerProfileViewModel.Instance;
            }
        }

        public MinerProfileDual() {
            this.DataContext = AppContext.MinerProfileViewModel.Instance;
            InitializeComponent();
            this.PopupDualCoinPool.Closed += Popup_Closed;
            this.PopupDualCoin.Closed += Popup_Closed;
            this.PopupDualCoinWallet.Closed += Popup_Closed;
            this.On<LocalContextVmsReInitedEvent>("本地上下文视图模型集刷新后刷新界面上的popup", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        if (Vm.MineWork != null) {
                            return;
                        }
                        if (this.PopupDualCoinPool != null) {
                            this.PopupDualCoinPool.IsOpen = false;
                            OpenDualCoinPoolPopup();
                        }
                        if (this.PopupDualCoin != null) {
                            this.PopupDualCoin.IsOpen = false;
                            OpenDualCoinPopup();
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
            if (Vm.CoinVm == null
                || Vm.CoinVm.CoinKernel == null
                || Vm.CoinVm.CoinKernel.CoinKernelProfile == null) {
                return;
            }
            CoinKernelProfileViewModel coinKernelProfileVm = Vm.CoinVm.CoinKernel.CoinKernelProfile;
            NTMinerRoot.Instance.MinerProfile.SetCoinKernelProfileProperty(coinKernelProfileVm.CoinKernelId, nameof(coinKernelProfileVm.DualCoinWeight), coinKernelProfileVm.DualCoinWeight);
            NTMinerRoot.RefreshArgsAssembly.Invoke();
        }

        #region OpenPopup

        private void OpenDualCoinPoolPopup() {
            var coinVm = Vm.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin;
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

        private void OpenDualCoinPopup() {
            if (Vm.CoinVm == null || Vm.CoinVm.CoinKernel == null) {
                return;
            }
            var popup = PopupDualCoin;
            popup.IsOpen = true;
            var selected = Vm.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin;
            popup.Child = new CoinSelect(
                new CoinSelectViewModel(Vm.CoinVm.CoinKernel.DualCoinGroup.DualCoinVms, selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (Vm.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin != selectedResult) {
                            Vm.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin = selectedResult;
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
            var coinVm = Vm.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin;
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

        private void KbButtonDualCoinPool_Clicked(object sender, RoutedEventArgs e) {
            OpenDualCoinPoolPopup();
            UserActionHappend();
        }

        private void KbButtonDualCoin_Clicked(object sender, RoutedEventArgs e) {
            OpenDualCoinPopup();
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
