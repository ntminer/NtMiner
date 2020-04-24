using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class SysDicPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "系统字典",
                IconName = "Icon_SysDic",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 1000,
                Height = 520
            }, ucFactory: (window) => new SysDicPage(), fixedSize: false);
        }

        public SysDicPageViewModel Vm { get; private set; }

        public SysDicPage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new SysDicPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<SysDicViewModel>(sender, e);
        }

        private void SysDicItemDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<SysDicItemViewModel>(sender, e);
        }
    }
}
