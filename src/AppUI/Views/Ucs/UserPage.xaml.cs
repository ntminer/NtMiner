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
                CloseVisible = Visibility.Visible
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
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < 30) {
                return;
            }
            if (dg.SelectedItem != null) {
                ((MineWorkViewModel)dg.SelectedItem).Edit.Execute(null);
            }
        }
    }
}
