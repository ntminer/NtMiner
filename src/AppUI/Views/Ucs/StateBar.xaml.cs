using NTMiner.Core.Gpus;
using NTMiner.Vms;
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
            var gpuSet = NTMinerRoot.Current.GpuSet;
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
    }
}
