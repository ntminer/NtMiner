using NTMiner.Vms;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerProfileIndex : UserControl {
        public MinerProfileIndexViewModel Vm { get; private set; }

        public MinerProfileIndex() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
#if DEBUG
            NTStopwatch.Start();
#endif
            this.Vm = new MinerProfileIndexViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded(window => {
                window.BuildEventPath<LocalContextReInitedEventHandledEvent>("上下文视图模型集刷新后刷新界面上的popup", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        UIThread.Execute(() => {
                            if (Vm.MinerProfile.MineWork != null) {
                                return;
                            }
                            if (this.PopupKernel.Child != null && this.PopupKernel.IsOpen) {
                                OpenKernelPopup();
                            }
                            if (this.PopupMainCoinPool.Child != null && this.PopupMainCoinPool.IsOpen) {
                                OpenMainCoinPoolPopup();
                            }
                            if (this.PopupMainCoinPool1.Child != null && this.PopupMainCoinPool1.IsOpen) {
                                OpenMainCoinPool1Popup();
                            }
                            if (this.PopupMainCoin != null && this.PopupMainCoin.IsOpen) {
                                OpenMainCoinPopup();
                            }
                            if (this.PopupMainCoinWallet != null && this.PopupMainCoinWallet.IsOpen) {
                                OpenMainCoinWalletPopup();
                            }
                        });
                    });
            });
#if DEBUG
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                NTMinerConsole.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
            }
#endif
        }

        private void DualCoinWeightSlider_LostFocus(object sender, RoutedEventArgs e) {
            if (Vm.MinerProfile.CoinVm == null
                || Vm.MinerProfile.CoinVm.CoinKernel == null
                || Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile == null) {
                return;
            }
            CoinKernelProfileViewModel coinKernelProfileVm = Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile;
            NTMinerContext.Instance.MinerProfile.SetCoinKernelProfileProperty(coinKernelProfileVm.CoinKernelId, nameof(coinKernelProfileVm.DualCoinWeight), coinKernelProfileVm.DualCoinWeight);
            NTMinerContext.RefreshArgsAssembly.Invoke("主界面上的双挖权重拖动条失去焦点时");
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
            CoinKernelSelectViewModel vm = null;
            // 如果服务器上下文刷新了则视图模型一定不等，因为上下文刷新后服务器视图模型会清空重建
            bool newVm = popup.Child == null || ((CoinKernelSelectViewModel)((CoinKernelSelect)popup.Child).DataContext).Coin != coinVm;
            if (newVm) {
                vm = new CoinKernelSelectViewModel(coinVm, selected, onOk: selectedResult => {
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
                };
            }
            if (popup.Child == null) {
                popup.Child = new CoinKernelSelect(vm);
            }
            else if (newVm) {
                ((CoinKernelSelect)popup.Child).DataContext = vm;
            }
            else {
                ((CoinKernelSelect)popup.Child).Vm.SelectedResult = selected;
            }
        }

        private void OpenMainCoinPoolPopup() {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null || coinVm.CoinProfile == null) {
                return;
            }
            var popup = PopupMainCoinPool;
            popup.IsOpen = true;
            var selected = coinVm.CoinProfile.MainCoinPool;
            PoolSelectViewModel vm = null;
            // 如果服务器上下文刷新了则视图模型一定不等，因为上下文刷新后服务器视图模型会清空重建
            bool newVm = popup.Child == null || ((PoolSelectViewModel)((PoolSelect)popup.Child).DataContext).Coin != coinVm;
            if (newVm) {
                vm = new PoolSelectViewModel(coinVm, selected, onOk: selectedResult => {
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
                };
            }
            if (popup.Child == null) {
                popup.Child = new PoolSelect(vm);
            }
            else if (newVm) {
                ((PoolSelect)popup.Child).DataContext = vm;
            }
            else {
                ((PoolSelect)popup.Child).Vm.SelectedResult = selected;
            }
        }

        private void OpenMainCoinPool1Popup() {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null || coinVm.CoinProfile == null) {
                return;
            }
            var popup = PopupMainCoinPool1;
            popup.IsOpen = true;
            var selected = coinVm.CoinProfile.MainCoinPool1;
            PoolSelectViewModel vm = null;
            // 如果服务器上下文刷新了则视图模型一定不等，因为上下文刷新后服务器视图模型会清空重建
            bool newVm = popup.Child == null || ((PoolSelectViewModel)((PoolSelect)popup.Child).DataContext).Coin != coinVm;
            if (newVm) {
                vm = new PoolSelectViewModel(coinVm, selected, onOk: selectedResult => {
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
                };
            }
            if (popup.Child == null) {
                popup.Child = new PoolSelect(vm);
            }
            else if (newVm) {
                ((PoolSelect)popup.Child).DataContext = vm;
            }
            else {
                ((PoolSelect)popup.Child).Vm.SelectedResult = selected;
            }
        }

        private void OpenMainCoinPopup() {
            var popup = PopupMainCoin;
            popup.IsOpen = true;
            var selected = Vm.MinerProfile.CoinVm;
            CoinSelectViewModel vm = null;
            // 如果服务器上下文刷新了则视图模型一定不等，因为上下文刷新后服务器视图模型会清空重建
            bool newVm = popup.Child == null || ((CoinSelectViewModel)((CoinSelect)popup.Child).DataContext).SelectedResult != selected;
            if (newVm) {
                vm = new CoinSelectViewModel(AppRoot.CoinVms.MainCoins.Where(a => a.IsSupported), selected, onOk: selectedResult => {
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
                };
            }
            if (popup.Child == null) {
                popup.Child = new CoinSelect(vm);
            }
            else if (newVm) {
                ((CoinSelect)popup.Child).DataContext = vm;
            }
            else {
                ((CoinSelect)popup.Child).Vm.SelectedResult = selected;
            }
        }

        private void OpenMainCoinWalletPopup() {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null || coinVm.CoinProfile == null) {
                return;
            }
            var popup = PopupMainCoinWallet;
            popup.IsOpen = true;
            var selected = coinVm.CoinProfile.SelectedWallet;
            bool isDualCoin = false;
            WalletSelectViewModel vm = null;
            // 如果服务器上下文刷新了则视图模型一定不等，因为上下文刷新后服务器视图模型会清空重建
            bool newVm = popup.Child == null || ((WalletSelectViewModel)((WalletSelect)popup.Child).DataContext).Coin != coinVm;
            if (newVm) {
                vm = new WalletSelectViewModel(coinVm, isDualCoin, selected, onOk: selectedResult => {
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
                };
            }
            if (popup.Child == null) {
                popup.Child = new WalletSelect(vm);
            }
            else if (newVm) {
                ((WalletSelect)popup.Child).DataContext = vm;
            }
        }

        #endregion

        private void KbButtonKernel_Clicked(object sender, RoutedEventArgs e) {
            if (Vm.MinerProfile.IsMining) {
                VirtualRoot.Out.ShowWarn("请先停止挖矿", header: "提示", autoHideSeconds: 3);
                return;
            }
            OpenKernelPopup();
            UserActionHappend();
            e.Handled = true;
        }

        private void KbButtonMainCoinPool_Clicked(object sender, RoutedEventArgs e) {
            if (Vm.MinerProfile.IsMining) {
                VirtualRoot.Out.ShowWarn("请先停止挖矿", header: "提示", autoHideSeconds: 3);
                return;
            }
            OpenMainCoinPoolPopup();
            UserActionHappend();
            e.Handled = true;
        }

        private void KbButtonMainCoin_Clicked(object sender, RoutedEventArgs e) {
            if (Vm.MinerProfile.IsMining) {
                VirtualRoot.Out.ShowWarn("请先停止挖矿", header: "提示", autoHideSeconds: 3);
                return;
            }
            OpenMainCoinPopup();
            UserActionHappend();
            e.Handled = true;
        }

        private void KbButtonMainCoinWallet_Clicked(object sender, RoutedEventArgs e) {
            if (Vm.MinerProfile.IsMining) {
                VirtualRoot.Out.ShowWarn("请先停止挖矿", header: "提示", autoHideSeconds: 3);
                return;
            }
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null) {
                return;
            }
            if (coinVm.Wallets.Count == 0) {
                coinVm.CoinProfile?.AddWallet.Execute(null);
            }
            else {
                OpenMainCoinWalletPopup();
            }
            e.Handled = true;
        }

        private static void UserActionHappend() {
            if (!DevMode.IsDevMode) {
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
            if (Vm.MinerProfile.IsMining) {
                VirtualRoot.Out.ShowWarn("请先停止挖矿", header: "提示", autoHideSeconds: 3);
                return;
            }
            OpenMainCoinPool1Popup();
            UserActionHappend();
            e.Handled = true;
        }

        private void BtnPopup_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            VirtualRoot.Execute(new TopmostCommand());
        }

        private void BtnPopup_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            VirtualRoot.Execute(new UnTopmostCommand());
        }
    }
}
