using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class VirtualMemory : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_VirtualMemory",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) => new VirtualMemory(), fixedSize: false);
        }

        private PropertyViewModel Vm {
            get {
                return (PropertyViewModel)this.DataContext;
            }
        }

        public VirtualMemory() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void VirtualMemorySlider_LostFocus(object sender, System.Windows.RoutedEventArgs e) {
            Vms.VirtualMemory.SetVirtualMemoryOfDrive();
        }
    }
}
