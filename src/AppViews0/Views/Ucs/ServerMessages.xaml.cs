using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class ServerMessages : UserControl {
        public ServerMessagesViewModel Vm { get; private set; }

        public ServerMessages() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new ServerMessagesViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded(window => {
                if (ClientAppType.IsMinerStudio) {
                    window.AddEventPath<Per1MinuteEvent>("周期刷新群控客户端的服务器消息集", LogEnum.DevConsole,
                    action: message => {
                        VirtualRoot.Execute(new LoadNewServerMessageCommand());
                    }, location: this.GetType());
                }
            });
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (!Vm.MainMenu.IsMinerStudioOuterAdminLogined) {
                return;
            }
            WpfUtil.DataGrid_EditRow<ServerMessageViewModel>(sender, e);
        }
    }
}
