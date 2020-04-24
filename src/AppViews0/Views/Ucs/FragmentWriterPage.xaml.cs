using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class FragmentWriterPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "命令行片段书写器",
                IconName = "Icon_FragmentWriter",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 1200,
                Height = 620
            }, 
            ucFactory: (window) => new FragmentWriterPage());
        }

        public FragmentWriterPageViewModel Vm { get; private set; }

        private FragmentWriterPage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new FragmentWriterPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<FragmentWriterViewModel>(sender, e);
        }
    }
}
