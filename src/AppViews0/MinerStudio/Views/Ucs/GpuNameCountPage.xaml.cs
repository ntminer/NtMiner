using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class GpuNameCountPage : UserControl {
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
