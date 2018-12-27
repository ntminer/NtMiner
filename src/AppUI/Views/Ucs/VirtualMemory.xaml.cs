using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class VirtualMemory : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "虚拟内存",
                IconName = "Icon_VirtualMemory",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) => new VirtualMemory(), fixedSize: true);
        }

        private EnviromentViewModel Vm {
            get {
                return (EnviromentViewModel)this.DataContext;
            }
        }

        public VirtualMemory() {
            InitializeComponent();
        }
    }
}
