using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Linq;
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
#if DEBUG
            NTStopwatch.Start();
#endif
            InitializeComponent();
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.OnLoaded((window) => {
                window.Activated += (object sender, EventArgs e) => {
                    Vm.OnPropertyChanged(nameof(Vm.IsAutoAdminLogon));
                    Vm.OnPropertyChanged(nameof(Vm.AutoAdminLogonToolTip));
                    Vm.OnPropertyChanged(nameof(Vm.IsRemoteDesktopEnabled));
                    Vm.OnPropertyChanged(nameof(Vm.RemoteDesktopToolTip));
                };
                window.AddEventPath<LocalIpSetInitedEvent>("本机IP集刷新后刷新状态栏", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => Vm.RefreshLocalIps);
                    }, location: this.GetType());
                window.AddEventPath<MinutePartChangedEvent>("时间的分钟部分变更过更新计时器显示", LogEnum.None,
                    action: message => {
                        UIThread.Execute(() => Vm.UpdateDateTime);
                    }, location: this.GetType());
                window.AddEventPath<Per1SecondEvent>("挖矿计时秒表", LogEnum.None,
                    action: message => {
                        DateTime now = DateTime.Now;
                        Vm.UpdateBootTimeSpan(now - NTMinerRoot.Instance.CreatedOn);
                        var mineContext = NTMinerRoot.Instance.LockedMineContext;
                        if (mineContext != null && mineContext.MineStartedOn != DateTime.MinValue) {
                            Vm.UpdateMineTimeSpan(now - mineContext.MineStartedOn);
                        }
                    }, location: this.GetType());
                window.AddEventPath<AppVersionChangedEvent>("发现了服务端新版本", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => () => {
                            Vm.SetCheckUpdateForeground(isLatest: EntryAssemblyInfo.CurrentVersion >= NTMinerRoot.ServerVersion);
                        });
                    }, location: this.GetType());
                window.AddEventPath<KernelSelfRestartedEvent>("内核自我重启时刷新计数器", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => () => {
                            Vm.OnPropertyChanged(nameof(Vm.KernelSelfRestartCountText));
                        });
                    }, location: this.GetType());
                window.AddEventPath<MineStartedEvent>("挖矿开始后将内核自我重启计数清零", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => () => {
                            Vm.OnPropertyChanged(nameof(Vm.KernelSelfRestartCountText));
                        });
                    }, location: this.GetType());
            });
            var gpuSet = NTMinerRoot.Instance.GpuSet;
            // 建议每张显卡至少对应4G虚拟内存，否则标红
            if (NTMinerRoot.OSVirtualMemoryMb < gpuSet.Count * 4) {
                BtnShowVirtualMemory.Foreground = WpfUtil.RedBrush;
            }
#if DEBUG
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
            }
#endif
        }

        private void BtnCheckUpdate_Click(object sender, RoutedEventArgs e) {
            if (this.IconCheckUpdate.Visibility == Visibility.Visible) {
                Process process = Process.GetProcessesByName(NTKeyword.NTMinerUpdaterProcessName).FirstOrDefault();
                if (process == null) {
                    this.IconCheckUpdate.Visibility = Visibility.Collapsed;
                    this.IconLoading.Visibility = Visibility.Visible;
                    // 这里的逻辑是每100毫秒检查一次升级器进程是否存在，每检查一次将loading图标
                    // 旋转30度，如果升级器进程存在了或者已经检查了3秒钟了则停止检查。
                    Interval.Start(
                        per: TimeSpan.FromMilliseconds(100),
                        perCallback: () => {
                            UIThread.Execute(() => () => {
                                ((RotateTransform)this.IconLoading.RenderTransform).Angle += 30;
                            });
                        },
                        stopCallback: () => {
                            UIThread.Execute(() => () => {
                                this.IconCheckUpdate.Visibility = Visibility.Visible;
                                this.IconLoading.Visibility = Visibility.Collapsed;
                                ((RotateTransform)this.IconLoading.RenderTransform).Angle = 0;
                            });
                        },
                        timeout: TimeSpan.FromSeconds(3),
                        requestStop: () => {
                            return Process.GetProcessesByName(NTKeyword.NTMinerUpdaterProcessName).FirstOrDefault() != null;
                        }
                    );
                }
            }
        }
    }
}
