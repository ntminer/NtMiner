using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class NTMinerWalletPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "NTMiner钱包",
                IconName = "Icon_Wallet",
                Width = 800,
                Height = 400,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => new NTMinerWalletPage(), fixedSize: true);
        }

        public NTMinerWalletPageViewModel Vm {
            get {
                return (NTMinerWalletPageViewModel)this.DataContext;
            }
        }

        public NTMinerWalletPage() {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<NTMinerWalletViewModel>(sender, e);
        }
    }
}
