using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class GroupPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Group",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 860,
                Height = 520
            }, ucFactory: (window) => new GroupPage(), fixedSize: false);
        }

        private GroupPageViewModel Vm {
            get {
                return (GroupPageViewModel)this.DataContext;
            }
        }

        public GroupPage() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(nameof(GroupPage), this.Resources);
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null) {
                ((GroupViewModel)dg.SelectedItem).Edit.Execute(null);
            }
        }
    }
}
