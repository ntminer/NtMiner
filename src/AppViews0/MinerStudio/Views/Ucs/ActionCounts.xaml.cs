using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class ActionCounts : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "WebApi Actions",
                IconName = "Icon_Action",
                Width = 800,
                Height = 700,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => new ActionCounts());
        }

        public ActionCountsViewModel Vm { get; private set; }

        public ActionCounts() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new ActionCountsViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void TbKeyword_LostFocus(object sender, RoutedEventArgs e) {
            ((ActionCountsViewModel)this.DataContext).Search.Execute(null);
        }

        private void TbKeyword_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                ((ActionCountsViewModel)this.DataContext).Keyword = this.TbKeyword.Text;
            }
        }
    }
}
