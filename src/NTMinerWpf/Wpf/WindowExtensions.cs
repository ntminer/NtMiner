using System.Windows;

namespace NTMiner.Wpf {
    public static class WindowExtensions {
        public static void MouseBottom(this Window window) {
            POINT pt;
            if (NativeMethods.GetCursorPos(out pt)) {
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                double left = pt.X - window.Width / 2;
                double top = pt.Y + 20;
                if (left < window.Owner.Left) {
                    left = window.Owner.Left;
                }
                if (top + window.Height > window.Owner.Top + window.Owner.Height) {
                    top += top + window.Height - window.Top - window.Owner.Height;
                }
                if (left + window.Width > window.Owner.Left + window.Owner.Width) {
                    left = window.Owner.Left + window.Owner.Width - window.Width;
                }
                if (top + 200 > window.Owner.Top + window.Owner.Height) {
                    top = window.Owner.Top + window.Owner.Height - 200;
                }
                window.Left = left;
                window.Top = top;
            }
        }

        public static bool? ShowDialogEx(this Window window) {
            bool? result;
            var owner = TopWindow.GetTopWindow();
            if (owner != window) {
                window.Owner = owner;
                double ownerOpacity = window.Owner.Opacity;
                window.Owner.Opacity = 0.6;
                result = window.ShowDialog();
                window.Owner.Opacity = ownerOpacity;
            }
            else {
                result = window.ShowDialog();
            }
            return result;
        }
    }
}
