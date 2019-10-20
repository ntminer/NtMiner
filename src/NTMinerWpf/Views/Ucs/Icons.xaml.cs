using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Icons : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "图标",
                IconName = "Icon_Icon",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 1440,
                Height = 800
            },
            ucFactory: (window) => new Icons());
        }

        public IconsViewModel Vm {
            get {
                return (IconsViewModel)this.DataContext;
            }
        }

        public Icons() {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
