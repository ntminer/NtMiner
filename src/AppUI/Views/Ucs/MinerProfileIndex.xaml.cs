using NTMiner.Vms;
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

        private void KbComboBox_DropDownOpened(object sender, System.EventArgs e) {
            ContainerWindow window = KernelSelect.ShowWindow(Vm.MinerProfile.CoinVm, Vm.MinerProfile.CoinVm.CoinKernel?.Kernel);
            Window parentWindow = Window.GetWindow(this);
            var control = ((FrameworkElement)sender);
            Point point = control.TransformToAncestor(parentWindow).Transform(new Point(0, control.Height));
            window.Left = parentWindow.Left + point.X;
            window.Top = parentWindow.Top + point.Y;
            window.Activate();
            VirtualRoot.Happened(new UserActionEvent());
        }
    }
}
