using NTMiner.Core;
using System.Diagnostics;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ToolboxViewModel : ViewModelBase {
        public ICommand SwitchRadeonGpu { get; private set; }
        public ICommand AtikmdagPatcher { get; private set; }
        public ICommand NavigateToNvidiaDriverWin10 { get; private set; }
        public ICommand NavigateToNvidiaDriverWin7 { get; private set; }
        public ICommand NavigateToAmdDriver { get; private set; }
        public ICommand RegCmdHere { get; private set; }

        public ToolboxViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.SwitchRadeonGpu = new DelegateCommand(() => {
                this.ShowDialog(message: $"确定运行吗？大概需要花费5到10秒钟时间看到结果", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new SwitchRadeonGpuCommand());
                }, icon: IconConst.IconConfirm);
            }, () => NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD);
            this.AtikmdagPatcher = new DelegateCommand(() => {
                VirtualRoot.Execute(new AtikmdagPatcherCommand());
            });
            this.NavigateToNvidiaDriverWin10 = new DelegateCommand(() => {
                Process.Start("https://www.geforce.cn/drivers/results/137770");
            });
            this.NavigateToNvidiaDriverWin7 = new DelegateCommand(() => {
                Process.Start("https://www.geforce.cn/drivers/results/137752");
            });
            this.NavigateToAmdDriver = new DelegateCommand(() => {
                Process.Start("https://www.amd.com/zh-hans/support");
            });
            this.RegCmdHere = new DelegateCommand(() => {
                this.ShowDialog(message: $"确定在windows右键上下文菜单中添加\"命令行\"菜单吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RegCmdHereCommand());
                }, icon: IconConst.IconConfirm);
            });
        }
    }
}
