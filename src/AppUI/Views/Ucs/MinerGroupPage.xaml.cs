using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerGroupPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_MinerGroup",
                Width = 600,
                Height = 400,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => new MinerGroupPage(), fixedSize: true);
        }

        public MinerGroupPageViewModel Vm {
            get {
                return (MinerGroupPageViewModel)this.DataContext;
            }
        }

        public MinerGroupPage() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null) {
                ((MinerGroupViewModel)dg.SelectedItem).Edit.Execute(null);
            }
        }
    }
}
