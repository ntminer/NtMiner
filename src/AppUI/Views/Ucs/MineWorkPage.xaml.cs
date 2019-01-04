using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MineWorkPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_MineWork",
                Width = 600,
                Height = 400,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => new MineWorkPage(), fixedSize: true);
        }

        public MineWorkPageViewModel Vm {
            get {
                return (MineWorkPageViewModel)this.DataContext;
            }
        }

        public MineWorkPage() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(nameof(MineWorkPage), this.Resources);
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null) {
                ((MineWorkViewModel)dg.SelectedItem).Edit.Execute(null);
            }
        }
    }
}
