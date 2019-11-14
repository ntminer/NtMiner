using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class LocalMessagesConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "消息配置",
                IconName = "Icon_Message",
                Width = 600,
                Height = 400,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) => new LocalMessagesConfig(), fixedSize: true);
        }

        public LocalMessagesConfigViewModel Vm {
            get {
                return (LocalMessagesConfigViewModel)this.DataContext;
            }
        }

        public LocalMessagesConfig() {
            InitializeComponent();
            VirtualRoot.Execute(new LoadKernelOutputKeywordCommand());
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_MouseDoubleClick<CoinViewModel>(sender, e);
        }
    }
}
