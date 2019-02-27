using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class GpusOverClock : UserControl {
        public GpusOverClockViewModel Vm {
            get {
                return (GpusOverClockViewModel)this.DataContext;
            }
        }

        public GpusOverClock() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && e.Source.GetType() == typeof(ScrollViewer)) {
                ScrollViewer scrollViewer = (ScrollViewer)sender;
                if (scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible) {
                    Point p = e.GetPosition(scrollViewer);
                    if (p.X > scrollViewer.ActualWidth - SystemParameters.ScrollWidth) {
                        return;
                    }
                }
                Window.GetWindow(this).DragMove();
                e.Handled = true;
            }
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
