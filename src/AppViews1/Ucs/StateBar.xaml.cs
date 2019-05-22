using NTMiner.Vms;
using System.Windows.Controls;
using System.Windows.Media;

namespace NTMiner.Views.Ucs {
    public partial class StateBar : UserControl {
        private StateBarViewModel Vm {
            get {
                return (StateBarViewModel)this.DataContext;
            }
        }

        public StateBar() {
            InitializeComponent();
            this.On<Per10SecondEvent>("周期轮播挖状态栏的矿信息和公告信息", LogEnum.None,
                action: message => {
                    if (Vm.IsNoticeVisible == System.Windows.Visibility.Visible) {
                        Vm.IsNoticeVisible = System.Windows.Visibility.Collapsed;
                    }
                    else {
                        Vm.IsNoticeVisible = System.Windows.Visibility.Visible;
                    }
                });
            this.On<ServerVersionChangedEvent>("发现了服务端新版本", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        if (NTMinerRoot.CurrentVersion.ToString() != NTMinerRoot.ServerVersion) {
                            Vm.CheckUpdateForeground = new SolidColorBrush(Colors.Red);
                        }
                        else {
                            Vm.CheckUpdateForeground = new SolidColorBrush(Colors.Black);
                        }
                    });
                });
        }
    }
}
