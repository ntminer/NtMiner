using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class UserPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_User",
                Width = 600,
                Height = 300,
                CloseVisible = Visibility.Visible,
                FooterText = "远程用户是供群控端访问挖矿端的用户"
            }, ucFactory: (window) => new UserPage(), fixedSize: true);
        }

        public UserPageViewModel Vm {
            get {
                return (UserPageViewModel)this.DataContext;
            }
        }

        public UserPage() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<UserViewModel>(sender, e);
        }
    }
}
