using NTMiner.Vms;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class StateBar : UserControl {
        private StateBarViewModel Vm {
            get {
                return (StateBarViewModel)this.DataContext;
            }
        }

        public StateBar() {
            InitializeComponent();
            if (Design.IsInDesignMode) {
                return;
            }
            this.RunOneceOnLoaded((window) => {
                window.Activated += (object sender, EventArgs e) => {
                    Vm.OnPropertyChanged(nameof(Vm.IsAutoAdminLogon));
                    Vm.OnPropertyChanged(nameof(Vm.AutoAdminLogonToolTip));
                    Vm.OnPropertyChanged(nameof(Vm.IsRemoteDesktopEnabled));
                    Vm.OnPropertyChanged(nameof(Vm.RemoteDesktopToolTip));
                };
                window.EventPath<LocalIpSetRefreshedEvent>("本机IP集刷新后刷新状态栏", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.RefreshLocalIps();
                        });
                    });
                window.EventPath<MinutePartChangedEvent>("时间的分钟部分变更过更新计时器显示", LogEnum.None,
                    action: message => {
                        Vm.UpdateDateTime();
                    });
                window.EventPath<Per1SecondEvent>("挖矿计时秒表", LogEnum.None,
                    action: message => {
                        DateTime now = DateTime.Now;
                        Vm.UpdateBootTimeSpan(now - NTMinerRoot.Instance.CreatedOn);
                        var mineContext = NTMinerRoot.Instance.CurrentMineContext;
                        if (mineContext != null) {
                            Vm.UpdateMineTimeSpan(now - mineContext.CreatedOn);
                        }
                    });
                window.EventPath<AppVersionChangedEvent>("发现了服务端新版本", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.SetCheckUpdateForeground(isLatest: MainAssemblyInfo.CurrentVersion >= NTMinerRoot.ServerVersion);
                        });
                    });
                window.EventPath<KernelSelfRestartedEvent>("内核自我重启时刷新计数器", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.OnPropertyChanged(nameof(Vm.KernelSelfRestartCountText));
                        });
                    });
                window.EventPath<MineStartedEvent>("挖矿开始后将内核自我重启计数清零", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.OnPropertyChanged(nameof(Vm.KernelSelfRestartCountText));
                        });
                    });
            });
            var gpuSet = NTMinerRoot.Instance.GpuSet;
            // 建议每张显卡至少对应4G虚拟内存，否则标红
            if (NTMinerRoot.OSVirtualMemoryMb < gpuSet.Count * 4) {
                BtnShowVirtualMemory.Foreground = Wpf.Util.RedBrush;
            }
        }

        private void BtnLocalIps_Click(object sender, MouseButtonEventArgs e) {
            AppStatic.ShowLocalIps.Execute(null);
            e.Handled = true;
        }
    }
}
