using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelInputPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核输入",
                IconName = "Icon_KernelInput",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 1200,
                Height = 620
            }, 
            ucFactory: (window) => new KernelInputPage());
        }

        private KernelInputPageViewModel Vm {
            get {
                return (KernelInputPageViewModel)this.DataContext;
            }
        }

        public KernelInputPage() {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_MouseDoubleClick<KernelInputViewModel>(sender, e);
        }
    }
}
