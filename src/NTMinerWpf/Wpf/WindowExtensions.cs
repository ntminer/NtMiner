using System.Windows;

namespace NTMiner.Wpf {
    public static class WindowExtensions {
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
