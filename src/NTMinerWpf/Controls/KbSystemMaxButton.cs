using System.Windows;
using System.Windows.Media;

namespace NTMiner.Controls {
    public class KbSystemMaxButton : KbSystemButton {
        private Window window;
        public KbSystemMaxButton() {
            Click += delegate {
                if (window.WindowState == WindowState.Normal) {
                    Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(window);
                } else if (window.WindowState == WindowState.Maximized) {
                    Microsoft.Windows.Shell.SystemCommands.RestoreWindow(window);
                }
            };
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            if (window == null) {
                window = Window.GetWindow(this);
                ResetIcon();
                window.StateChanged += (sender, e) => {
                    ResetIcon();
                };
            }
        }

        private void ResetIcon() {
            if (window.WindowState == WindowState.Maximized) {
                this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Maxed"];
            }
            else {
                this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Max"];
            }
        }
    }
}
