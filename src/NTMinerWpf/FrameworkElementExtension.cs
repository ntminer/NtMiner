using System;
using System.Windows;

namespace NTMiner {
    public static class FrameworkElementExtension {
        public static void OnLoaded(this FrameworkElement uc, Action<Window> onLoad, Action<Window> onUnload = null) {
            uc.Loaded += (sender, e) => {
                if (uc.Resources == null) {
                    uc.Resources = new ResourceDictionary();
                }
                // 因为如果Uc在Popup中的话每次IsOpen都会触发Loaded事件，所以这里加个标志位
                if (!uc.Resources.Contains("_isFirstLoaded")) {
                    uc.Resources.Add("_isFirstLoaded", string.Empty);
                    onLoad?.Invoke(Window.GetWindow(uc));
                }
            };
            if (onUnload != null) {
                uc.Unloaded += (sender, e) => {
                    // 因为如果Uc在Popup中的话每次IsOpen都会触发Loaded事件，所以这里加个标志位
                    if (!uc.Resources.Contains("_isFirstUnloaded")) {
                        uc.Resources.Add("_isFirstUnloaded", string.Empty);
                        onUnload?.Invoke(Window.GetWindow(uc));
                    }
                };
            }
        }
    }
}
