using NTMiner.Vms;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NTMiner.Views.Ucs {
    public partial class MinerProfileIndex : UserControl {
        private MinerProfileIndexViewModel Vm {
            get {
                return (MinerProfileIndexViewModel)this.DataContext;
            }
        }

        public MinerProfileIndex() {
            InitializeComponent();
            this.PopupKernel.Closed += (object sender, System.EventArgs e) => {
                this.PopupKernel.Child = null;
            };
            this.PopupMainCoinPool.Closed += (object sender, System.EventArgs e) => {
                this.PopupMainCoinPool.Child = null;
            };
            this.PopupDualCoinPool.Closed += (object sender, System.EventArgs e) => {
                this.PopupDualCoinPool.Child = null;
            };
            this.PopupMainCoin.Closed += (object sender, System.EventArgs e) => {
                this.PopupMainCoin.Child = null;
            };
            this.PopupDualCoin.Closed += (object sender, System.EventArgs e) => {
                this.PopupDualCoin.Child = null;
            };
            this.PopupMainCoinWallet.Closed += (object sender, System.EventArgs e) => {
                this.PopupMainCoinWallet.Child = null;
            };
            this.PopupDualCoinWallet.Closed += (object sender, System.EventArgs e) => {
                this.PopupDualCoinWallet.Child = null;
            };
            NTMinerRoot.Current.OnReRendMinerProfile += Current_OnReRendMinerProfile;
        }

        protected override void OnRender(DrawingContext drawingContext) {
            Window.GetWindow(this).Closing += (object sender, System.ComponentModel.CancelEventArgs e) => {
                // 对于挖矿端来说MinerProfileIndex实例是唯一的，但对于群控客户端的作业来说不是，所以这里需要-=
                NTMinerRoot.Current.OnReRendMinerProfile -= Current_OnReRendMinerProfile;
            };
            base.OnRender(drawingContext);
        }

        private void Current_OnReRendMinerProfile() {
            UIThread.Execute(() => {
                if (Vm.MinerProfile.MineWork != null) {
                    return;
                }
                if (this.PopupKernel.Child != null) {
                    OpenKernelPopup();
                }
                if (this.PopupMainCoinPool.Child != null) {
                    OpenMainCoinPoolPopup();
                }
                if (this.PopupDualCoinPool != null) {
                    OpenDualCoinPoolPopup();
                }
                if (this.PopupMainCoin != null) {
                    OpenMainCoinPopup();
                }
                if (this.PopupDualCoin != null) {
                    OpenDualCoinPopup();
                }
                if (this.PopupMainCoinWallet != null) {
                    OpenMainCoinWalletPopup();
                }
                if (this.PopupDualCoinWallet != null) {
                    OpenDualCoinWalletPopup();
                }
            });
        }

        private void DualCoinWeightSlider_LostFocus(object sender, System.Windows.RoutedEventArgs e) {
            if (Vm.MinerProfile.CoinVm == null
                || Vm.MinerProfile.CoinVm.CoinKernel == null
                || Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile == null) {
                return;
            }
            CoinKernelProfileViewModel coinKernelProfileVm = Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile;
            NTMinerRoot.Current.MinerProfile.SetCoinKernelProfileProperty(coinKernelProfileVm.CoinKernelId, nameof(coinKernelProfileVm.DualCoinWeight), coinKernelProfileVm.DualCoinWeight);
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
                new CoinKernelSelectViewModel(coinVm, selected, onSelectedChanged: selectedResult => {
                    if (selectedResult != null) {
                        coinVm.CoinKernel = selectedResult;
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
                new PoolSelectViewModel(coinVm, selected, onSelectedChanged: selectedResult => {
                    if (selectedResult != null) {
                        coinVm.CoinProfile.MainCoinPool = selectedResult;
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
                new PoolSelectViewModel(coinVm, selected, onSelectedChanged: selectedResult => {
                    if (selectedResult != null) {
                        coinVm.CoinProfile.DualCoinPool = selectedResult;
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
                new CoinSelectViewModel(Vm.CoinVms.MainCoins.Where(a => a.IsSupported), selected, onSelectedChanged: selectedResult => {
                    if (selectedResult != null) {
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
                new CoinSelectViewModel(Vm.MinerProfile.CoinVm.CoinKernel.DualCoinGroup.DualCoinVms, selected, onSelectedChanged: selectedResult => {
                    if (selectedResult != null) {
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
                new WalletSelectViewModel(coinVm, isDualCoin, selected, onSelectedChanged: selectedResult => {
                    if (selectedResult != null) {
                        coinVm.CoinProfile.SelectedWallet = selectedResult;
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
                new WalletSelectViewModel(coinVm, isDualCoin, selected, onSelectedChanged: selectedResult => {
                    if (selectedResult != null) {
                        coinVm.CoinProfile.SelectedDualCoinWallet = selectedResult;
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
            VirtualRoot.Happened(new UserActionEvent());
        }

        private void KbButtonMainCoinPool_Clicked(object sender, RoutedEventArgs e) {
            OpenMainCoinPoolPopup();
            VirtualRoot.Happened(new UserActionEvent());
        }

        private void KbButtonDualCoinPool_Clicked(object sender, RoutedEventArgs e) {
            OpenDualCoinPoolPopup();
            VirtualRoot.Happened(new UserActionEvent());
        }

        private void KbButtonMainCoin_Clicked(object sender, RoutedEventArgs e) {
            OpenMainCoinPopup();
            VirtualRoot.Happened(new UserActionEvent());
        }

        private void KbButtonDualCoin_Clicked(object sender, RoutedEventArgs e) {
            OpenDualCoinPopup();
            VirtualRoot.Happened(new UserActionEvent());
        }

        private void KbButtonMainCoinWallet_Clicked(object sender, RoutedEventArgs e) {
            OpenMainCoinWalletPopup();
            VirtualRoot.Happened(new UserActionEvent());
        }

        private void KbButtonDualCoinWallet_Clicked(object sender, RoutedEventArgs e) {
            OpenDualCoinWalletPopup();
            VirtualRoot.Happened(new UserActionEvent());
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
