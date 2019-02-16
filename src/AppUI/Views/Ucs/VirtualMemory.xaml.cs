using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class VirtualMemory : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_VirtualMemory",
                CloseVisible = System.Windows.Visibility.Visible,
                Width = 800,
                MinWidth = 800,
                Height = 360,
                MinHeight = 420
            }, ucFactory: (window) => new VirtualMemory(), fixedSize: false);
        }

        private DriveSet Vm {
            get {
                return (DriveSet)this.DataContext;
            }
        }

        public VirtualMemory() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
