using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerProfileIndex : UserControl {
        public static string ViewId = nameof(MinerProfileIndex);

        private MinerProfileViewModel Vm {
            get {
                return (MinerProfileViewModel)this.DataContext;
            }
        }

        public MinerProfileIndex() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void DualCoinWeightSlider_LostFocus(object sender, System.Windows.RoutedEventArgs e) {
            if (Vm.CoinVm == null
                || Vm.CoinVm.CoinKernel == null) {
                return;
            }
            CoinKernelProfileViewModel coinKernelProfileVm = Vm.CoinVm.CoinKernel.CoinKernelProfile;
            NTMinerRoot.Current.MinerProfile.SetCoinKernelProfileProperty(coinKernelProfileVm.CoinKernelId, nameof(coinKernelProfileVm.DualCoinWeight), coinKernelProfileVm.DualCoinWeight);
            NTMinerRoot.RefreshArgsAssembly.Invoke();
        }

        private void KbComboBox_DropDownOpened(object sender, System.EventArgs e) {
            VirtualRoot.Happened(new UserActionEvent());
        }
    }
}
