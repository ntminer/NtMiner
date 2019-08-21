using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class SpeedTable : UserControl {
        private SpeedTableViewModel Vm {
            get {
                return (SpeedTableViewModel)this.DataContext;
            }
        }

        public SpeedTable() {
            InitializeComponent();
        }

        public void ShowOrHideOverClock(bool isShow) {
            if (isShow) {
                Vm.IsOverClockVisible = Visibility.Visible;
            }
            else {
                Vm.IsOverClockVisible = Visibility.Collapsed;
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
