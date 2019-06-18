using NTMiner.Core.Gpus;
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
            this.RunOneceOnLoaded(() => {
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
                window.On<AppVersionChangedEvent>("发现了服务端新版本", LogEnum.DevConsole,
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
                window.On<KernelSelfRestartedEvent>("内核自我重启时刷新计数器", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.OnPropertyChanged(nameof(Vm.KernelSelfRestartCountText));
                        });
                    });
            });
            var gpuSet = NTMinerRoot.Instance.GpuSet;
            // 建议每张显卡至少对应4G虚拟内存，否则标红
            if (NTMinerRoot.OSVirtualMemoryMb < gpuSet.Count * 4) {
                BtnShowVirtualMemory.Foreground = new SolidColorBrush(Colors.Red);
            }
            if (!gpuSet.Has20NCard()) {
                string nvDriverVersion = gpuSet.DriverVersion;
                if (double.TryParse(nvDriverVersion, out double driverNum) && driverNum >= 400) {
                    TextBlockDriverVersion.Foreground = new SolidColorBrush(Colors.Red);
                    TextBlockDriverVersion.ToolTip = "如果没有20系列的N卡，挖矿建议使用3xx驱动。";
                }
            }
        }
    }
}
