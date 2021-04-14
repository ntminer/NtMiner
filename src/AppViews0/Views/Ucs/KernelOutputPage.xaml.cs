using NTMiner.Vms;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputPage : UserControl {
        public static void ShowWindow(KernelOutputViewModel selectedKernelOutputVm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核输出",
                IconName = "Icon_KernelOutput",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 1360,
                Height = 720
            }, 
            ucFactory: (window) => new KernelOutputPage(selectedKernelOutputVm));
        }

        public KernelOutputPageViewModel Vm { get; private set; }

        public KernelOutputPage(KernelOutputViewModel selectedKernelOutputVm) {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new KernelOutputPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            if (selectedKernelOutputVm != null) {
                Vm.CurrentKernelOutputVm = selectedKernelOutputVm;
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<KernelOutputViewModel>(sender, e);
        }

        private void KernelOutputTranslaterDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<KernelOutputTranslaterViewModel>(sender, e);
        }

        private void KernelOutputKeywordDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<KernelOutputKeywordViewModel>(sender, e);
        }

        private bool _isFirstSelectedOutputKeywords = true;
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (((TabControl)sender).SelectedItem == TabItemOutputKeywords) {
                if (_isFirstSelectedOutputKeywords) {
                    VirtualRoot.Execute(new LoadKernelOutputKeywordCommand());
                }
                _isFirstSelectedOutputKeywords = false;
            }
        }
    }
}
