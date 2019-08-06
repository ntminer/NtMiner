using NTMiner.Vms;
using System;
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
                // 时间事件是在WPF UI线程的，所以这里不用考虑访问UI线程创建的Vm对象的问题
                window.On<MinutePartChangedEvent>("时间的分钟部分变更过更新计时器显示", LogEnum.None,
                    action: message => {
                        Vm.UpdateDateTime();
                    });
                window.On<Per1SecondEvent>("挖矿计时秒表", LogEnum.None,
                    action: message => {
                        DateTime now = DateTime.Now;
                        Vm.UpdateBootTimeSpan(now - NTMinerRoot.Instance.CreatedOn);
                        if (NTMinerRoot.IsAutoStart && VirtualRoot.SecondCount <= Vm.MinerProfile.AutoStartDelaySeconds && !NTMinerRoot.IsAutoStartCanceled) {
                            return;
                        }
                        var mineContext = NTMinerRoot.Instance.CurrentMineContext;
                        if (mineContext != null) {
                            Vm.UpdateMineTimeSpan(now - mineContext.CreatedOn);
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
                window.On<AppVersionChangedEvent>("发现了服务端新版本", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.SetCheckUpdateForeground(isLatest: NTMinerRoot.CurrentVersion.ToString() == NTMinerRoot.ServerVersion);
                        });
                    });
                window.On<KernelSelfRestartedEvent>("内核自我重启时刷新计数器", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.OnPropertyChanged(nameof(Vm.KernelSelfRestartCountText));
                        });
                    });
                window.On<MineStartedEvent>("挖矿开始后将内核自我重启计数清零", LogEnum.DevConsole,
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
    }
}
