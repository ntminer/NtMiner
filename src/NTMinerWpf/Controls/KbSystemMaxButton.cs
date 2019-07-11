using System.Windows;
using System.Windows.Media;

namespace NTMiner.Controls {
    public class KbSystemMaxButton : KbSystemButton {
        private Window window;
        public KbSystemMaxButton() {
            this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Max"];
            Click += delegate {
                if (window.WindowState == WindowState.Normal) {
                    Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(window);
                    this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Maxed"];
                    window.Show();
                } else if (window.WindowState == WindowState.Maximized) {
                    Microsoft.Windows.Shell.SystemCommands.RestoreWindow(window);
                    this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Max"];
                }
            };
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            if (Design.IsInDesignMode) {
                return;
            }
            if (window == null) {
                window = Window.GetWindow(this);
                if (window.WindowState == WindowState.Maximized) {
                    this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Maxed"];
                }
                else {
                    this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Max"];
                }
                window.StateChanged += (object sender, System.EventArgs e) => {
                    if (window.WindowState == WindowState.Maximized) {
                        this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Maxed"];
                    }
                    else if (window.WindowState == WindowState.Normal) {
                        this.Icon = (StreamGeometry)Application.Current.Resources["Icon_Max"];
                    }
                };
            }
        }
    }
}
