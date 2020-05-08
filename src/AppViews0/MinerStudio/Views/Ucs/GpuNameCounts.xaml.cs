using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class GpuNameCounts : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "Gpu名称统计",
                IconName = "Icon_Gpu",
                Width = 600,
                Height = 700,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => new GpuNameCounts(), fixedSize: true);
        }

        public GpuNameCountsViewModel Vm { get; private set; }

        public GpuNameCounts() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new GpuNameCountsViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void TbKeyword_LostFocus(object sender, RoutedEventArgs e) {
            Vm.Search.Execute(null);
        }

        private void TbKeyword_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                Vm.Keyword = this.TbKeyword.Text;
            }
        }
    }
}
