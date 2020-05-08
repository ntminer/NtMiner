using NTMiner.MinerStudio.Vms;
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
                Width = 1400,
                Height = 700,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => new GpuNamePage());
        }

        public GpuNamePageViewModel Vm { get; private set; }

        public GpuNamePage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new GpuNamePageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }
    }
}
