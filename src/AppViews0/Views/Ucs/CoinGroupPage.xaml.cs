using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinGroupPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "币组",
                IconName = "Icon_Group",
                IsMaskTheParent = true,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 660,
                Height = 420
            }, ucFactory: (window) => new CoinGroupPage(), fixedSize: false);
        }

        private CoinGroupPageViewModel Vm {
            get {
                return (CoinGroupPageViewModel)this.DataContext;
            }
        }

        public CoinGroupPage() {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<GroupViewModel>(sender, e);
        }
    }
}
