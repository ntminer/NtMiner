using NTMiner.Core;
using NTMiner.Views;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ToolboxViewModel : ViewModelBase {
        public ICommand SwitchRadeonGpu { get; private set; }

        public ToolboxViewModel() {
            this.SwitchRadeonGpu = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"确定运行吗？运行可能花费几秒钟时间", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new SwitchRadeonGpuCommand());
                }, icon: IconConst.IconConfirm);
            }, () => NTMinerRoot.Current.GpuSet.GpuType == GpuType.AMD);
        }
    }
}
