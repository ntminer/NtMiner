using System.Windows;

namespace NTMiner.Controls {
    public class KbSystemMaxButton : KbSystemButton {
        private Window targetWindow;
        public KbSystemMaxButton() {
            Click += delegate {
                if (targetWindow == null) {
                    targetWindow = Window.GetWindow(this);
                }
                if (targetWindow.WindowState == WindowState.Normal) {
                    targetWindow.WindowState = WindowState.Maximized;
                } else if (targetWindow.WindowState == WindowState.Maximized) {
                    targetWindow.WindowState = WindowState.Normal;
                }
            };
        }
    }
}
