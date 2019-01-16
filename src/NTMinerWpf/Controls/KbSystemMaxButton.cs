using System.Windows;
using System.Windows.Media;

namespace NTMiner.Controls {
    public class KbSystemMaxButton : KbSystemButton {
        private Window targetWindow;
        public KbSystemMaxButton() {
            this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Max"];
            Click += delegate {
                if (targetWindow.WindowState == WindowState.Normal) {
                    targetWindow.WindowState = WindowState.Maximized;
                    this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Maxed"];
                } else if (targetWindow.WindowState == WindowState.Maximized) {
                    targetWindow.WindowState = WindowState.Normal;
                    this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Max"];
                }
            };
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            if (Design.IsInDesignMode) {
                return;
            }
            if (targetWindow == null) {
                targetWindow = Window.GetWindow(this);
                targetWindow.StateChanged += (object sender, System.EventArgs e) => {
                    if (targetWindow.WindowState == WindowState.Maximized) {
                        this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Maxed"];
                    }
                    else if (targetWindow.WindowState == WindowState.Normal) {
                        this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Max"];
                    }
                };
            }
        }
    }
}
