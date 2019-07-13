using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class OuterProperty : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "属性",
                IconName = "Icon_Property",
                Width = 700,
                Height = AppStatic.MainWindowHeight,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new OuterProperty();
                return uc;
            }, fixedSize: false);
        }

        private OuterPropertyViewModel Vm {
            get {
                return (OuterPropertyViewModel)this.DataContext;
            }
        }

        public OuterProperty() {
            InitializeComponent();
            if (Design.IsInDesignMode) {
                return;
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ItemsControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
