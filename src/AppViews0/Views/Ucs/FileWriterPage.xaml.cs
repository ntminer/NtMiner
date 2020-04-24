using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class FileWriterPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "文件书写器",
                IconName = "Icon_FileWriter",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 1200,
                Height = 620
            }, 
            ucFactory: (window) => new FileWriterPage());
        }

        public FileWriterPageViewModel Vm { get; private set; }

        private FileWriterPage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new FileWriterPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<FileWriterViewModel>(sender, e);
        }
    }
}
