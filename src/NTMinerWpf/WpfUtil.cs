using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner {
    public static class WpfUtil {
        public static ChildType FindVisualChild<ChildType>(DependencyObject obj) where ChildType : DependencyObject {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++) {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is ChildType) {
                    return child as ChildType;
                }
                else {
                    ChildType childOfChildren = FindVisualChild<ChildType>(child);
                    if (childOfChildren != null) {
                        return childOfChildren;
                    }
                }
            }
            return null;
        }

        public static void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && e.Source.GetType() == typeof(ScrollViewer)) {
                ScrollViewer scrollViewer = (ScrollViewer)sender;
                if (scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible) {
                    Point p = e.GetPosition(scrollViewer);
                    if (p.X > scrollViewer.ActualWidth - SystemParameters.ScrollWidth) {
                        return;
                    }
                }
                Window.GetWindow(scrollViewer).DragMove();
                e.Handled = true;
            }
        }

        public static void DataGrid_MouseDoubleClick<T>(object sender, MouseButtonEventArgs e) where T : IEditableViewModel {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < 30) {
                return;
            }
            if (dg.SelectedItem != null) {
                ((T)dg.SelectedItem).Edit.Execute(FormType.Edit);
            }
        }
    }
}
