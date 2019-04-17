using NTMiner.Vms;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace NTMiner.Views.Ucs {
    public partial class StateBar : UserControl {
        public static string ViewId = nameof(StateBar);

        private StateBarViewModel Vm {
            get {
                return (StateBarViewModel)this.DataContext;
            }
        }

        public StateBar() {
            InitializeComponent();
            var gpuSet = NTMinerRoot.Current.GpuSet;
            if (NTMinerRoot.OSVirtualMemoryMb < gpuSet.Count * 4) {
                BtnShowVirtualMemory.Foreground = new SolidColorBrush(Colors.Red);
            }
            if (gpuSet.GpuType == GpuType.NVIDIA) {
                if (gpuSet.All(a => !Is20NCard(a.Name))) {
                    string nvDriverVersion = string.Empty;
                    var driverVersion = gpuSet.Properties.FirstOrDefault(a => a.Code == "DriverVersion");
                    if (driverVersion != null && driverVersion.Value != null) {
                        nvDriverVersion = driverVersion.Value.ToString();
                    }
                    double driverNum;
                    if (double.TryParse(nvDriverVersion, out driverNum) && driverNum >= 400) {
                        TextBlockDriverVersion.Foreground = new SolidColorBrush(Colors.Red);
                        TextBlockDriverVersion.ToolTip = "如果没有20系列的显卡，挖矿建议使用3xx驱动。";
                    }
                }
            }
        }

        private static bool Is20NCard(string cardName) {
            if (string.IsNullOrEmpty(cardName)) {
                return false;
            }
            string[] nv20Cards = new string[] { "2060", "2070", "2080" };
            foreach (var nv20 in nv20Cards) {
                if (cardName.Contains(nv20)) {
                    return true;
                }
            }
            return false;
        }
    }
}
