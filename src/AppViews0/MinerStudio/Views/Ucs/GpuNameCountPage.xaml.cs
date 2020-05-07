using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class GpuNameCountPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "Gpu名称统计",
                IconName = "Icon_Gpu",
                Width = 500,
                Height = 700,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => new GpuNameCountPage(), fixedSize: true);
        }

        public GpuNameCountPageViewModel Vm { get; private set; }

        public GpuNameCountPage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new GpuNameCountPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }
    }
}
