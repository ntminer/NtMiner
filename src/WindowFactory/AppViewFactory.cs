using NTMiner.MinerServer;
using System.Windows;

namespace NTMiner {
    public static class AppViewFactory {
        private static readonly IAppViewFactory _factory = new AppView2Factory();

        public static Window CreateSplashWindow() {
            return _factory.CreateSplashWindow();
        }

        public static void ShowMainWindow(bool isToggle) {
            _factory.ShowMainWindow(isToggle);
        }

        public static void ShowMainWindow(Application app, NTMinerAppType appType) {
            _factory.ShowMainWindow(app, appType);
        }

        public static void Link() {
            _factory.Link();
        }
    }
}
