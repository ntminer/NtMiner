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
                    VirtualRoot.Execute(new RefreshIsRemoteDesktopEnabledCommand());
                };
                window.AddEventPath<LocalIpSetInitedEvent>("本机IP集刷新后刷新状态栏", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => Vm.RefreshLocalIps());
                    }, location: this.GetType());
                window.AddEventPath<MinutePartChangedEvent>("时间的分钟部分变更过更新计时器显示", LogEnum.None,
                    action: message => {
                        UIThread.Execute(() => Vm.UpdateDateTime());
                    }, location: this.GetType());
                window.AddEventPath<Per1SecondEvent>("挖矿计时秒表", LogEnum.None,
                    action: message => {
                        DateTime now = DateTime.Now;
                        Vm.UpdateBootTimeSpan(now - NTMinerContext.Instance.CreatedOn);
                        var mineContext = NTMinerContext.Instance.LockedMineContext;
                        if (mineContext != null && mineContext.MineStartedOn != DateTime.MinValue) {
                            Vm.UpdateMineTimeSpan(now - mineContext.MineStartedOn);
                        }
                    }, location: this.GetType());
                window.AddEventPath<AppVersionChangedEvent>("发现了服务端新版本", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.SetCheckUpdateForeground(isLatest: EntryAssemblyInfo.CurrentVersion >= NTMinerContext.ServerVersion);
                        });
                    }, location: this.GetType());
                window.AddEventPath<KernelSelfRestartedEvent>("内核自我重启时刷新计数器", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.OnPropertyChanged(nameof(Vm.KernelSelfRestartCountText));
                        });
                    }, location: this.GetType());
                window.AddEventPath<MineStartedEvent>("挖矿开始后将内核自我重启计数清零", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.OnPropertyChanged(nameof(Vm.KernelSelfRestartCountText));
                        });
                    }, location: this.GetType());
                window.AddCmdPath<RefreshIsRemoteDesktopEnabledCommand>(LogEnum.DevConsole, 
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.RefreshIsRemoteDesktopEnabled();
                        });
                    }, location: this.GetType());
            });
            var gpuSet = NTMinerContext.Instance.GpuSet;
            // 建议每张显卡至少对应4G虚拟内存，否则标红
            if (VirtualRoot.DriveSet.OSVirtualMemoryMb < gpuSet.Count * 4) {
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
            if (this.UpdateIcon.Visibility == Visibility.Visible) {
                Process process = Process.GetProcessesByName(NTKeyword.NTMinerUpdaterProcessName).FirstOrDefault();
                if (process == null) {
                    this.UpdateIcon.Visibility = Visibility.Collapsed;
                    this.LoadingIcon.Visibility = Visibility.Visible;
                    // 这里的逻辑是每100毫秒检查一次升级器进程是否存在，每检查一次将loading图标
                    // 旋转30度，如果升级器进程存在了或者已经检查了3秒钟了则停止检查。
                    VirtualRoot.SetInterval(
                        per: TimeSpan.FromMilliseconds(100),
                        perCallback: () => {
                            UIThread.Execute(() => {
                                ((RotateTransform)this.LoadingIcon.RenderTransform).Angle += 30;
                            });
                        },
                        stopCallback: () => {
                            UIThread.Execute(() => {
                                this.UpdateIcon.Visibility = Visibility.Visible;
                                this.LoadingIcon.Visibility = Visibility.Collapsed;
                                ((RotateTransform)this.LoadingIcon.RenderTransform).Angle = 0;
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

        private void PanelEprice_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.ShowInputDialog("电价", Vm.MinerProfile.EPrice.ToString("f2"), "￥ / 度", eprice => {
                if (!double.TryParse(eprice, out _)) {
                    return "电价必须是数字";
                }
                return string.Empty;
            }, onOk: eprice => {
                Vm.MinerProfile.EPrice = double.Parse(eprice);
            });
        }
    }
}
