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
            var selectedKernel = coinVm.CoinKernel?.Kernel;
            if (selectedKernel == null) {
                return;
            }
            bool isExceptedCoin = false;
            PopupKernel.Child = new KernelSelect(
                new KernelSelectViewModel(coinVm, isExceptedCoin, selectedKernel, onSelectedKernelChanged: selectedResult=> {
                    coinVm.CoinKernel = coinVm.CoinKernels.FirstOrDefault(a => a.Kernel == selectedResult);
                    PopupKernel.IsOpen = false;
                }));
            PopupKernel.IsOpen = true;
            VirtualRoot.Happened(new UserActionEvent());
        }
    }
}
