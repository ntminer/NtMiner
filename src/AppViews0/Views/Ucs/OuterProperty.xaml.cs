using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class OuterProperty : UserControl {
        public OuterPropertyViewModel Vm { get; private set; }

        public OuterProperty() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new OuterPropertyViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            if (WpfUtil.IsInDesignMode) {
                return;
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
