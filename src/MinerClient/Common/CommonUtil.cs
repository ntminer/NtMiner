using System;
using System.IO;
using System.Reflection;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
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
                if (!pathVar.EndsWith(";")) {
                    pathVar += ";" + Path.Combine(Environment.CurrentDirectory, "Common");
                }
                else {
                    pathVar += Path.Combine(Environment.CurrentDirectory, "Common");
                }
                Environment.SetEnvironmentVariable("PATH", pathVar);
                Write.DevDebug(pathVar);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
