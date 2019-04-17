using NTMiner.Vms;
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
            if (NTMinerRoot.OSVirtualMemoryMb < NTMinerRoot.Current.GpuSet.Count * 4) {
                BtnShowVirtualMemory.Foreground = new SolidColorBrush(Colors.Red);
            }
        }
    }
}
