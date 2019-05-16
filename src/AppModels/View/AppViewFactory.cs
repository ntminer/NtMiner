using NTMiner.MinerServer;
using System.Reflection;
using System.Text;
using System.Windows;

namespace NTMiner.View {
    public static class AppViewFactory {
        private static IAppViewFactory _innerFactory;
        private static readonly object _locker = new object();
        // 这里自定义了视图程序集的加载
        private static IAppViewFactory InnerFactory {
            get {
                if (_innerFactory == null) {
                    lock (_locker) {
                        if (_innerFactory == null) {
                            string appViewsName = "AppViews1";
                            var assembly = Assembly.GetEntryAssembly();
                            using (var stream = assembly.GetManifestResourceStream("NTMiner.Views.AppViewsName=")) {
                                byte[] data = new byte[stream.Length];
                                stream.Read(data, 0, data.Length);
                                appViewsName = Encoding.UTF8.GetString(data);
                            }
                            using (var stream = assembly.GetManifestResourceStream($"NTMiner.Views.{appViewsName}.dll")) {
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
