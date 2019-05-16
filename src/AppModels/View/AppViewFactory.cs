using NTMiner.MinerServer;
using System.Reflection;
using System.Windows;

namespace NTMiner.View {
    public static class AppViewFactory {
        private static IAppViewFactory _innerFactory;
        private static readonly object _locker = new object();
        private static IAppViewFactory InnerFactory {
            get {
                if (_innerFactory == null) {
                    lock (_locker) {
                        if (_innerFactory == null) {
                            var assembly = Assembly.GetEntryAssembly();
                            using (var stream = assembly.GetManifestResourceStream("NTMiner.Views.AppViews1.dll")) {
                                byte[] data = new byte[stream.Length];
                                stream.Read(data, 0, data.Length);
                                assembly = Assembly.Load(data);
                                _innerFactory = (IAppViewFactory)assembly.CreateInstance("NTMiner.Views.AppViewFactory");
                            }
                        }
                    }
                }
                return _innerFactory;
            }
        }

        public static void ShowMainWindow(bool isToggle) {
            InnerFactory.ShowMainWindow(isToggle);
        }

        public static void ShowMainWindow(Application app, NTMinerAppType appType) {
            InnerFactory.ShowMainWindow(app, appType);
        }

        public static Window CreateSplashWindow() {
            return InnerFactory.CreateSplashWindow();
        }

        public static void Link() {
            InnerFactory.Link();
        }
    }
}
