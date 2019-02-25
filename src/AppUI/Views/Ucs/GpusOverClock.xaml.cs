using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class GpusOverClock : UserControl {
        public static void ShowWindow(string appType) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_GpuOverClock",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) => new GpusOverClock(appType), fixedSize: true);
        }

        public GpusOverClockViewModel Vm {
            get {
                return (GpusOverClockViewModel)this.DataContext;
            }
        }

        public GpusOverClock(string appType) {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
