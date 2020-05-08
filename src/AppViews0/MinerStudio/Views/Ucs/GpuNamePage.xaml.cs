using NTMiner.Views;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class GpuNamePage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "Gpu名",
                IconName = "Icon_Gpu",
                Width = 1000,
                Height = 700,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => new GpuNamePage());
        }

        public GpuNamePage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            InitializeComponent();
        }
    }
}
