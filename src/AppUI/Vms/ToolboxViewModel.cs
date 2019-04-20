using NTMiner.Core;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ToolboxViewModel : ViewModelBase {
        public ICommand SwitchRadeonGpu { get; private set; }

        public ToolboxViewModel() {
            this.SwitchRadeonGpu = new DelegateCommand(() => {
                VirtualRoot.Execute(new SwitchRadeonGpuCommand());
            }, () => NTMinerRoot.Current.GpuSet.GpuType == GpuType.AMD);
        }
    }
}
