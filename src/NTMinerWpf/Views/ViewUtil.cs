using NTMiner.Wpf;
using System;
using System.Windows;

namespace NTMiner.Views {
    public static class ViewUtil {
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
    }
}
