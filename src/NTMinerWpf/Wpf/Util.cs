using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Wpf {
    public static class Util {
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

        public static EventHandler ChangeNotiCenterWindowLocation(Window window) {
            return (sender, e) => {
                NotiCenterWindow.Instance.Left = window.Left + (window.Width - NotiCenterWindow.Instance.Width) / 2;
                NotiCenterWindow.Instance.Top = window.Top + 10;
            };
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
            if (p.Y < dg.ColumnHeaderHeight) {
                return;
            }
            if (dg.SelectedItem != null) {
                ((T)dg.SelectedItem).Edit.Execute(FormType.Edit);
            }
        }
    }
}
