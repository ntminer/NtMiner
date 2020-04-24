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
                Width = 1800,
                Height = 400,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => new WsServerNodePage(), fixedSize: false);
        }

        public WsServerNodePageViewModel Vm { get; private set; }

        public WsServerNodePage() {
            this.Vm = new WsServerNodePageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded(window => {
                window.AddEventPath<Per20SecondEvent>("外网群控服务器节点列表页面打开着时周期刷新", LogEnum.None, action: message => {
                    Vm.Refresh();
                }, this.GetType());
            });
        }
    }
}
