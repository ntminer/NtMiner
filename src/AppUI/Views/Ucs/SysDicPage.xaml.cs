using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class SysDicPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "字典",
                IconName = "Icon_SysDic",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 860,
                Height = 520
            }, ucFactory: (window) => new SysDicPage(), fixedSize: false);
        }

        private SysDicPageViewModel Vm {
            get {
                return (SysDicPageViewModel)this.DataContext;
            }
        }

        public SysDicPage() {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null) {
                ((SysDicViewModel)dg.SelectedItem).Edit.Execute(null);
            }
        }

        private void SysDicItemDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null) {
                SysDicItemViewModel poolVm = (SysDicItemViewModel)dg.SelectedItem;
                poolVm.Edit.Execute(null);
            }
        }
    }
}
