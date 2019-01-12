using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_KernelOutput",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 860,
                Height = 520
            }, 
            ucFactory: (window) => new KernelOutputPage());
        }

        private KernelOutputPageViewModel Vm {
            get {
                return (KernelOutputPageViewModel)this.DataContext;
            }
        }

        public KernelOutputPage() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (Vm.CurrentKernelOutputVm != null) {
                Vm.CurrentKernelOutputVm.Edit.Execute(null);
            }
        }
    }
}
