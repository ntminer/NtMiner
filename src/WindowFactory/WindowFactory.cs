using NTMiner.MinerServer;
using System.Windows;

namespace NTMiner {
    public static class WindowFactory {
        private static readonly IWindowFactory _factory = new AnotherWindowFactory();

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
