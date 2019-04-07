using NTMiner.Vms;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerProfileIndex : UserControl {
        public static string ViewId = nameof(MinerProfileIndex);

        private MinerProfileIndexViewModel Vm {
            get {
                return (MinerProfileIndexViewModel)this.DataContext;
            }
        }

        public MinerProfileIndex() {
            InitializeComponent();
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

        private void KbButtonKernel_Clicked(object sender, RoutedEventArgs e) {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null) {
                return;
            }
            var popup = PopupKernel;
            var selected = coinVm.CoinKernel;
            popup.Child = new CoinKernelSelect(
                new CoinKernelSelectViewModel(coinVm, selected, onSelectedChanged: selectedResult=> {
                    if (selectedResult != null) {
                        coinVm.CoinKernel = selectedResult;
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
            popup.IsOpen = true;
            VirtualRoot.Happened(new UserActionEvent());
        }

        private void KbButtonMainCoinPool_Clicked(object sender, RoutedEventArgs e) {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null) {
                return;
            }
            var popup = PopupMainCoinPool;
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
            popup.IsOpen = true;
            VirtualRoot.Happened(new UserActionEvent());
        }

        private void KbButtonDualCoinPool_Clicked(object sender, RoutedEventArgs e) {
            var coinVm = Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin;
            if (coinVm == null) {
                return;
            }
            var popup = PopupDualCoinPool;
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
            popup.IsOpen = true;
            VirtualRoot.Happened(new UserActionEvent());
        }

        private void KbButtonMainCoin_Clicked(object sender, RoutedEventArgs e) {
            var popup = PopupMainCoin;
            var selected = Vm.MinerProfile.CoinVm;
            popup.Child = new CoinSelect(
                new CoinSelectViewModel(Vm.CoinVms.MainCoins, selected, onSelectedChanged: selectedResult => {
                    if (selectedResult != null) {
                        Vm.MinerProfile.CoinVm = selectedResult;
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
            popup.IsOpen = true;
            VirtualRoot.Happened(new UserActionEvent());
        }

        private void KbButtonDualCoin_Clicked(object sender, RoutedEventArgs e) {
            if (Vm.MinerProfile.CoinVm == null || Vm.MinerProfile.CoinVm.CoinKernel == null) {
                return;
            }
            var popup = PopupDualCoin;
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
            popup.IsOpen = true;
            VirtualRoot.Happened(new UserActionEvent());
        }
    }
}
