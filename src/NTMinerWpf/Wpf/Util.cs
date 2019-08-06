using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Wpf {
    public static class Util {
        public static readonly SolidColorBrush TransparentBrush = new SolidColorBrush(Colors.Transparent);
        public static readonly SolidColorBrush WhiteBrush = new SolidColorBrush(Colors.White);
        public static readonly SolidColorBrush BlackBrush = new SolidColorBrush(Colors.Black);
        public static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Colors.Green);
        public static readonly SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);

        // 这个方法没有放在InputWindow类型中是为了去除编译时对MahaApps.Metro的依赖供AppModels使用
        public static void ShowInputDialog(
            string title,
            string text,
            Func<string, string> check,
            Action<string> onOk) {
            Window window = new InputWindow(title, text, check, onOk);
            if (window.Owner != null) {
                window.MouseBottom();
                double ownerOpacity = window.Owner.Opacity;
                window.Owner.Opacity = 0.6;
                window.ShowDialog();
                window.Owner.Opacity = ownerOpacity;
            }
            else {
                window.ShowDialog();
            }
        }

        public static void RunAsAdministrator() {
            ProcessStartInfo startInfo = new ProcessStartInfo {
                FileName = VirtualRoot.AppFileFullName,
                Arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1)),
                Verb = "runas"
            };
            Process.Start(startInfo);
            Application.Current.Shutdown();
        }

        public static ScrollViewer GetScrollViewer(this FlowDocumentScrollViewer element) {
            if (element == null) {
                throw new ArgumentNullException(nameof(element));
            }
            return element.Template?.FindName("PART_ContentHost", element) as ScrollViewer;
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
                if (scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible) {
                    Point p = e.GetPosition(scrollViewer);
                    if (p.Y > scrollViewer.ActualHeight - SystemParameters.ScrollHeight) {
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
            if (p.Y < dg.ColumnHeaderHeight) {
                return;
            }
            if (dg.SelectedItem != null) {
                ((T)dg.SelectedItem).Edit.Execute(FormType.Edit);
            }
        }
    }
}
