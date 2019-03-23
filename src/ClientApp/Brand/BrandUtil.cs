using System;
using System.Reflection;

namespace NTMiner.Brand {
    public static class BrandUtil {
        private static Guid _kernelBrandId;
        private static bool _isKernelBrandIdFirst = true;
        private static object _kernelBrandLocker = new object();
        public static Guid KernelBrandId {
            get {
                if (_isKernelBrandIdFirst) {
                    lock (_kernelBrandLocker) {
                        if (_isKernelBrandIdFirst) {
                            Type type = typeof(BrandUtil);
                            Assembly assembly = type.Assembly;
                            using (var stream = assembly.GetManifestResourceStream(type, "KernelBrand.id")) {
                                byte[] data = new byte[stream.Length];
                                NTMinerRoot.KernelBrandRaw = data;
                                stream.Read(data, 0, data.Length);
                                string word = System.Text.Encoding.UTF8.GetString(data);
                                word = word.Substring("KernelBrandId".Length, word.Length - "KernelBrandId".Length * 2);
                                _kernelBrandId = Guid.Parse(word);
                            }
                            _isKernelBrandIdFirst = false;
                        }
                    }
                }
                return _kernelBrandId;
            }
        }
    }
}
