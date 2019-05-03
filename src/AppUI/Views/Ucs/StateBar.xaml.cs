using NTMiner.Core.Gpus;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace NTMiner.Views.Ucs {
    public partial class StateBar : UserControl {
        private StateBarViewModel Vm {
            get {
                return (StateBarViewModel)this.DataContext;
            }
        }

        private readonly List<Bus.IDelegateHandler> _handlers = new List<Bus.IDelegateHandler>();
        public StateBar() {
            InitializeComponent();
            VirtualRoot.On<Per1SecondEvent>("挖矿计时秒表", LogEnum.None,
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
                }).AddToCollection(_handlers);
            VirtualRoot.On<ServerVersionChangedEvent>("发现了服务端新版本", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        if (NTMinerRoot.CurrentVersion.ToString() != NTMinerRoot.ServerVersion) {
                            Vm.CheckUpdateForeground = new SolidColorBrush(Colors.Red);
                        }
                        else {
                            Vm.CheckUpdateForeground = new SolidColorBrush(Colors.Black);
                        }
                    });
                }).AddToCollection(_handlers);
            this.Unloaded += StateBar_Unloaded;
            var gpuSet = NTMinerRoot.Instance.GpuSet;
            // 建议每张显卡至少对应4G虚拟内存，否则标红
            if (NTMinerRoot.OSVirtualMemoryMb < gpuSet.Count * 4) {
                BtnShowVirtualMemory.Foreground = new SolidColorBrush(Colors.Red);
            }
            if (!gpuSet.Has20NCard()) {
                string nvDriverVersion = gpuSet.DriverVersion;
                double driverNum;
                if (double.TryParse(nvDriverVersion, out driverNum) && driverNum >= 400) {
                    TextBlockDriverVersion.Foreground = new SolidColorBrush(Colors.Red);
                    TextBlockDriverVersion.ToolTip = "如果没有20系列的N卡，挖矿建议使用3xx驱动。";
                }
            }
        }

        private void StateBar_Unloaded(object sender, System.Windows.RoutedEventArgs e) {
            foreach (var handler in _handlers) {
                VirtualRoot.UnPath(handler);
            }
        }
    }
}
