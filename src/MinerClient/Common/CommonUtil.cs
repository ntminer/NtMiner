using System;
using System.IO;
using System.Reflection;

namespace NTMiner.Common {
    public static class CommonUtil {
        public static void ExtractResource() {
            try {
                Type type = typeof(CommonUtil);
                Assembly assembly = type.Assembly;
                string[] fileNames = new string[] {
                    "cudart32_80.dll",
                    "cudart64_80.dll",
                    "cudart64_91.dll",
                    "libcurl.dll",
                    "msvcp120.dll",
                    "msvcp140.dll",
                    "msvcr110.dll",
                    "msvcr120.dll",
                    "OpenCL.dll",
                    "vcruntime140.dll"
                };
                foreach (var fileName in fileNames) {
                    using (var stream = assembly.GetManifestResourceStream(type, fileName)) {
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        File.WriteAllBytes(Path.Combine(SpecialPath.CommonDirFullName, fileName), data);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
