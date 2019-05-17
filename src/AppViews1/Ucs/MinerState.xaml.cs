using NTMiner.Vms;
using System;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerState : UserControl {
        private MinerStateViewModel Vm {
            get {
                return (MinerStateViewModel)this.DataContext;
            }
        }

        public MinerState() {
            InitializeComponent();
            this.On<Per1SecondEvent>("挖矿计时秒表", LogEnum.None,
                action: message => {
                    DateTime now = DateTime.Now;
                    Vm.StateBarVm.BootTimeSpan = now - NTMinerRoot.Instance.CreatedOn;
                    if (NTMinerRoot.IsAutoStart && VirtualRoot.SecondCount <= 10 && !NTMinerRoot.IsAutoStartCanceled) {
                        return;
                    }
                    var mineContext = NTMinerRoot.Instance.CurrentMineContext;
                    if (mineContext != null) {
                        Vm.StateBarVm.MineTimeSpan = now - mineContext.CreatedOn;
                        if (!Vm.StateBarVm.MinerProfile.IsMining) {
                            Vm.StateBarVm.MinerProfile.IsMining = true;
                        }
                    }
                    else {
                        if (Vm.StateBarVm.MinerProfile.IsMining) {
                            Vm.StateBarVm.MinerProfile.IsMining = false;
                        }
                    }
                });
        }
    }
}
