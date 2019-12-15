using System;
using System.Windows;

namespace NTMiner {
    public static class FrameworkElementExtension {
        public static void RunOneceOnLoaded(this FrameworkElement uc, Action<Window> onLoad, Action<Window> onUnload = null) {
            uc.Loaded += (sender, e) => {
                onLoad?.Invoke(Window.GetWindow(uc));
            };
            if (onUnload != null) {
                uc.Unloaded += (sender, e) => {
                    onUnload?.Invoke(Window.GetWindow(uc));
                };
            }
        }
    }
}
