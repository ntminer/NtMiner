using NTMiner.Views;
using System.Windows;

namespace NTMiner {
    public class AnotherWindowFactory : DefaultWindowFactory {
        protected override Window CreateMainWindow() {
            return new MainWindow();
        }

        public override Window CreateSplashWindow() {
            return new SplashWindow();
        }

        public override void Link() {
            
        }
    }
}
