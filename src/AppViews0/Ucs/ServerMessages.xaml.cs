using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class ServerMessages : UserControl {
        private ServerMessagesViewModel Vm {
            get {
                return (ServerMessagesViewModel)this.DataContext;
            }
        }

        public ServerMessages() {
            InitializeComponent();
            this.RunOneceOnLoaded(window => {
                if (VirtualRoot.IsMinerStudio) {
                    window.AddEventPath<Per1MinuteEvent>("周期刷新群控客户端的服务器消息集", LogEnum.DevConsole,
                    action: message => {
                        VirtualRoot.Execute(new LoadNewServerMessageCommand());
                    }, location: this.GetType());
                }
            });
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (!DevMode.IsDevMode) {
                return;
            }
            WpfUtil.DataGrid_EditRow<ServerMessageViewModel>(sender, e);
        }
    }
}
