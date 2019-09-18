using NTMiner.Views;
using System.Windows;

namespace NTMiner.Wpf {
    public static class WindowExtensions {
        /// <summary>
        /// 基于鼠标位置放置窗口
        /// </summary>
        /// <param name="window"></param>
        public static void MousePosition(this Window window) {
            if (window.Owner == null) {
                return;
            }
            POINT pt;
            if (NativeMethods.GetCursorPos(out pt)) {
                var width = window.Width.Equals(double.NaN) ? 400 : window.Width;
                var height = window.Height.Equals(double.NaN) ? 200 : window.Height;
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                double left = pt.X - width / 2;
                double top = pt.Y + 20;
                if (left < window.Owner.Left) {
                    left = window.Owner.Left;
                }
                var ownerTop = window.Owner.Top;
                var ownerLeft = window.Owner.Left;
                if (window.Owner.WindowState == WindowState.Maximized) {
                    ownerTop = 0;
                    ownerLeft = 0;
                }
                var over = top + height - ownerTop - window.Owner.Height;
                if (over > 0) {
                    top = pt.Y - height - 20;
                }
                over = left + width - ownerLeft - window.Owner.Width;
                if (over > 0) {
                    left -= over;
                }
                window.Left = left;
                window.Top = top;
            }
        }

        public static bool? ShowDialogEx(this Window window) {
            bool? result;
            if (window.Owner == null) {
                var owner = TopWindow.GetTopWindow();
                if (owner != window) {
                    window.Owner = owner;
                }
            }
            if (window.Owner != null) {
                if (window.Owner is IMaskWindow maskWindow) {
                    maskWindow.ShowMask();
                    result = window.ShowDialog();
                    maskWindow.HideMask();
                }
                else {
                    double ownerOpacity = window.Owner.Opacity;
                    window.Owner.Opacity = 0.6;
                    result = window.ShowDialog();
                    window.Owner.Opacity = ownerOpacity;
                }
            }
            else {
                result = window.ShowDialog();
            }
            return result;
        }
    }
}
