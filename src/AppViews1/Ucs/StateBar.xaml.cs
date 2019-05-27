using NTMiner.Vms;
using System;
using System.Windows;
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
            this.Loaded += (sender, e) => {
                var window = Window.GetWindow(this);
                window.On<Per1SecondEvent>("挖矿计时秒表", LogEnum.None,
                    action: message => {
                        DateTime now = DateTime.Now;
                        Vm.BootTimeSpan = now - NTMinerRoot.Instance.CreatedOn;
                        if (NTMinerRoot.IsAutoStart && VirtualRoot.SecondCount <= 10 && !NTMinerRoot.IsAutoStartCanceled) {
                            return;
                        }
                        var mineContext = NTMinerRoot.Instance.CurrentMineContext;
                        if (mineContext != null) {
                            Vm.MineTimeSpan = now - mineContext.CreatedOn;
                            if (!Vm.MinerProfile.IsMining) {
                                Vm.MinerProfile.IsMining = true;
                            }
                        }
                        else {
                            if (Vm.MinerProfile.IsMining) {
                                Vm.MinerProfile.IsMining = false;
                            }
                        }
                    });
                window.On<Per10SecondEvent>("周期轮播挖状态栏的矿信息和公告信息", LogEnum.None,
                    action: message => {
                        if (Vm.IsNoticeVisible == Visibility.Visible) {
                            Vm.IsNoticeVisible = Visibility.Collapsed;
                        }
                        else {
                            Vm.IsNoticeVisible = Visibility.Visible;
                        }
                    });
                window.On<ServerVersionChangedEvent>("发现了服务端新版本", LogEnum.DevConsole,
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
            };
        }
    }
}
