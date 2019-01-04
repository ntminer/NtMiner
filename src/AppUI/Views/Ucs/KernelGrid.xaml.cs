using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelGrid : UserControl {
        public KernelPageViewModel Vm {
            get {
                return (KernelPageViewModel)this.DataContext;
            }
        }

        public KernelGrid() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(nameof(KernelGrid), this.Resources);
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null) {
                ((KernelViewModel)dg.SelectedItem).Edit.Execute(null);
            }
        }
    }
}
