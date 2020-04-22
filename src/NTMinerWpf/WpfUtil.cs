using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner {
    public static class WpfUtil {
        // SolidColorBrush是DependencyObject，必须在UT显存创建，所以这里提供一个什么也不干的Init方法
        // 只是为了触发静态构造函数从而确保SolidColorBrush是在UI显存构建的。
        public static void Init() { }
        // 一些ViewModel中有SolidColorBrush类型的DependencyObject，而ViewModel可能不是在UI线程创建的，所以就要
        // 要求ViewModel不能自己创建SolidColorBrush对象而必须从WpfUtil引用SolidColorBrush对象，从而避免异常。
        public static readonly SolidColorBrush TransparentBrush = new SolidColorBrush(Colors.Transparent);
        public static readonly SolidColorBrush WhiteBrush = new SolidColorBrush(Colors.White);
        public static readonly SolidColorBrush BlackBrush = new SolidColorBrush(Colors.Black);
        public static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Colors.Green);
        public static readonly SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);
        public static readonly SolidColorBrush LightRed = new SolidColorBrush(Color.FromRgb(0xFF, 0xCC, 0x00));
        public static readonly SolidColorBrush BlueBrush = new SolidColorBrush(Colors.Blue);
        public static readonly SolidColorBrush AxisForeground = new SolidColorBrush(Color.FromRgb(0x38, 0x52, 0x63));
        public static readonly SolidColorBrush SelectedBackground = new SolidColorBrush(Color.FromRgb(0x04, 0x35, 0x5B));
        public static readonly SolidColorBrush SelectedForeground = new SolidColorBrush(Color.FromRgb(0x2C, 0xA2, 0xFC));
        public static readonly SolidColorBrush SelectedBorderColor = new SolidColorBrush(Color.FromRgb(0x2C, 0xA2, 0xFC));


        public static readonly bool IsInDesignMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
        public static bool IsDevMode {
            get {
                if (IsInDesignMode || DevMode.IsDevMode) {
                    return true;
                }
                return false;
            }
        }

        public static Visibility IsDevModeVisible {
            get {
                if (IsDevMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static Window GetTopWindow() {
            IntPtr hwnd = SafeNativeMethods.GetForegroundWindow();
            if (hwnd == IntPtr.Zero) {
                return null;
            }

            return GetWindowFromHwnd(hwnd);
        }

        private static Window GetWindowFromHwnd(IntPtr hwnd) {
            HwndSource hds = HwndSource.FromHwnd(hwnd);
            if (hds == null) {
                return Application.Current.MainWindow;
            }
            if (hds.RootVisual is Window) {
                return (Window)hds.RootVisual;
            }
            return Application.Current.MainWindow;
        }

        // 这个方法没有放在InputWindow类型中是为了去除编译时对MahaApps.Metro的依赖供AppModels使用
        public static void ShowInputDialog(
            string title,
            string text,
            string tail,
            Func<string, string> check,
            Action<string> onOk,
            bool isPassword = false) {
            Window window = new InputWindow(title, text, tail, check, onOk, isPassword);
            window.MousePosition();
            window.ShowSoftDialog();
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

        public static void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e, bool isLeftBar = false) {
            if (isLeftBar) {
                if (e.LeftButton == MouseButtonState.Pressed && e.Source.GetType() == typeof(ScrollViewer)) {
                    ScrollViewer scrollViewer = (ScrollViewer)sender;
                    Point p = e.GetPosition(scrollViewer);
                    if (p.X < SystemParameters.ScrollWidth) {
                        return;
                    }
                    if (scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible) {
                        p = e.GetPosition(scrollViewer);
                        if (p.Y > scrollViewer.ActualHeight - SystemParameters.ScrollHeight) {
                            return;
                        }
                    }
                    Window.GetWindow(scrollViewer).DragMove();
                    e.Handled = true;
                }
            }
            else {
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
        }

        public static void DataGrid_MouseDoubleClick<T>(object sender, MouseButtonEventArgs e, Action<T> callback) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < dg.ColumnHeaderHeight) {
                return;
            }
            if (dg.SelectedItem != null) {
                callback?.Invoke(((T)dg.SelectedItem));
            }
        }

        public static void DataGrid_EditRow<T>(object sender, MouseButtonEventArgs e) where T : IEditableViewModel {
            DataGrid_MouseDoubleClick<T>(sender, e, t => {
                t.Edit.Execute(FormType.Edit);
            });
        }
    }
}
