using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class SpeedTable : UserControl {
        public Visibility IsOverClockVisible {
            get { return (Visibility)GetValue(IsOverClockVisibleProperty); }
            set { SetValue(IsOverClockVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsOverClockVisibleProperty =
            DependencyProperty.Register(nameof(IsOverClockVisible), typeof(Visibility), typeof(SpeedTable), new PropertyMetadata(Visibility.Collapsed));

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
                this.IsOverClockVisible = Visibility.Visible;
            }
            else {
                this.IsOverClockVisible = Visibility.Collapsed;
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
