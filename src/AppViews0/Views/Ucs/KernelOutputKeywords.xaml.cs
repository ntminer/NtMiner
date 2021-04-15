using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputKeywords : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核频道设置：基于关键字从内核输出中订阅消息",
                IconName = "Icon_Message",
                Width = 800,
                Height = 400,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) => {
                window.Owner = WpfUtil.GetTopWindow();
                return new KernelOutputKeywords();
            }, fixedSize: false);
        }

        public KernelOutputKeywordsViewModel Vm { get; private set; }

        public KernelOutputKeywords() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new KernelOutputKeywordsViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded(window => {
                window.BuildEventPath<KernelOutputKeywordLoadedEvent>("刷新Vm内存", LogEnum.None, this.GetType(), PathPriority.BelowNormal, message => {
                    this.Vm.KernelOutputVm?.OnPropertyChanged(nameof(KernelOutputViewModel.KernelOutputKeywords));
                });
            });
            VirtualRoot.Execute(new LoadKernelOutputKeywordCommand());
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<KernelOutputKeywordViewModel>(sender, e);
        }
    }
}
