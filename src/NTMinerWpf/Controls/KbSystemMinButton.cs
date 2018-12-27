using System.Windows;

namespace NTMiner.Controls {
    public class KbSystemMinButton : KbSystemButton {
        Window targetWindow;
        public KbSystemMinButton() {
            Click += delegate {
                if (targetWindow == null) {
                    targetWindow = Window.GetWindow(this);
                }
                targetWindow.WindowState = WindowState.Minimized;
            };
        }
    }
}
