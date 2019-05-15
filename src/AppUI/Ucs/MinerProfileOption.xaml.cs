using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerProfileOption : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "选项",
                IconName = "Icon_MinerProfile",
                Width = 450,
                Height = 360,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => new MinerProfileOption(), fixedSize: true);
        }

        public AppContext.MinerProfileViewModel Vm {
            get {
                return (AppContext.MinerProfileViewModel)this.DataContext;
            }
        }

        public MinerProfileOption() {
            this.DataContext = AppContext.Instance.MinerProfileVm;
            InitializeComponent();
            if (VirtualRoot.IsMinerStudio) {
                this.GroupSystemSetting.Visibility = Visibility.Collapsed;
            }
            else {
                this.GroupSystemSetting.Visibility = Visibility.Visible;
            }
        }

        private void ButtonHotKey_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key >= System.Windows.Input.Key.A && e.Key <= System.Windows.Input.Key.Z) {
                Vm.HotKey = e.Key.ToString();
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
