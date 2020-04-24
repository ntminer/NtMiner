using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputKeywords : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核频道设置：基于关键字从内核输出中订阅消息",
                IconName = "Icon_Message",
                Width = 600,
                Height = 400,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) => {
                window.Owner = WpfUtil.GetTopWindow();
                return new KernelOutputKeywords();
            }, fixedSize: true);
        }

        public KernelOutputKeywordsViewModel Vm { get; private set; }

        public KernelOutputKeywords() {
            this.Vm = new KernelOutputKeywordsViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            VirtualRoot.Execute(new LoadKernelOutputKeywordCommand());
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<KernelOutputKeywordViewModel>(sender, e);
        }
    }
}
