using System;
using System.IO;
using System.Reflection;

namespace NTMiner.Common {
    public static class CommonUtil {
        private static bool _isFirstCall = true;
        public static void SetCommonDirectory() {
            if (!_isFirstCall) {
                return;
            }
            _isFirstCall = false;
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
                    assembly.ExtractManifestResource(type, fileName, Path.Combine(SpecialPath.CommonDirFullName, fileName));
                }
                // Set working directory to exe
                var path = Path.GetDirectoryName(SpecialPath.CommonDirFullName);
                if (path != null) {
                    Environment.CurrentDirectory = path;
                }
                // Add common folder to path for launched processes
                var pathVar = Environment.GetEnvironmentVariable("PATH");
                pathVar += ";" + Path.Combine(Environment.CurrentDirectory, "Common");
                Environment.SetEnvironmentVariable("PATH", pathVar);
                Write.DevDebug(pathVar);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
