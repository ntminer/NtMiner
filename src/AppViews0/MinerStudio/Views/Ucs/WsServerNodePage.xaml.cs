using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class WsServerNodePage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "外网群控websocket服务器节点",
                IconName = "Icon_Server",
                Width = 1590,
                Height = 600,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => new WsServerNodePage(), fixedSize: false);
        }

        public WsServerNodePageViewModel Vm { get; private set; }

        public WsServerNodePage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new WsServerNodePageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded(window => {
                window.BuildEventPath<Per20SecondEvent>("外网群控服务器节点列表页面打开着时周期刷新", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                    Vm.DoRefresh();
                });
            });
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_MouseDoubleClick<WsServerNodeStateViewModel>(sender, e, rowVm => {
                rowVm.RemoteDesktop.Execute(null);
            });
        }
    }
}
