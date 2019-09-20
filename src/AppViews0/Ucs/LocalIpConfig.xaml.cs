using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class LocalIpConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "IP设置",
                IconName = "Icon_Ip",
                Width = 465,
                FooterVisible = Visibility.Collapsed,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new LocalIpConfig();
                LocalIpConfigViewModel vm = (LocalIpConfigViewModel)uc.DataContext;
                uc.ItemsControl.MouseDown += (object sender, MouseButtonEventArgs e)=> {
                    if (e.LeftButton == MouseButtonState.Pressed) {
                        window.DragMove();
                    }
                };
                return uc;
            }, fixedSize: true);
        }

        public LocalIpConfigViewModel Vm {
            get {
                return (LocalIpConfigViewModel)this.DataContext;
            }
        }

        private LocalIpConfig() {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
