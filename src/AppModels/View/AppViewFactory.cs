using System;
using System.Reflection;

namespace NTMiner.View {
    public static class AppViewFactory {
        private static IAppViewFactory _instance = null;
        private static readonly object _locker = new object();
        public static IAppViewFactory Instance {
            get {
                if (_instance == null) {
                    lock (_locker) {
                        if (_instance == null) {
                            Type type = typeof(AppViewFactory);
                            var assembly = type.Assembly;
                            using (var stream = assembly.GetManifestResourceStream(type, "AppViews2.dll")) {
                                byte[] data = new byte[stream.Length];
                                stream.Read(data, 0, data.Length);
                                assembly = Assembly.Load(data);
                                _instance = (IAppViewFactory)assembly.CreateInstance("NTMiner.AppViewFactory");
                            }
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
