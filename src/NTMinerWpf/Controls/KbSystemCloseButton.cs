using System.Windows;
using System.Windows.Media;

namespace NTMiner.Controls {
    public class KbSystemCloseButton : KbSystemButton {
        private Window targetWindow;
        public KbSystemCloseButton() {
            KbSystemButtonHoverBackground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

            Click += delegate {
                if (targetWindow == null) {
                    targetWindow = Window.GetWindow(this);
                }
                targetWindow.Close();
            };
        }
    }
}
