using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.Windows {
    public static class WindowsUtil {
        private const string _blockWAUResourceName = "BlockWAU.bat";
        private static readonly string _blockWAUFileFullName = Path.Combine(TempPath.TempDirFullName, _blockWAUResourceName);

        public static Task BlockWAU() {
            return Task.Factory.StartNew(() => {
                ExtractBlockWAUManifestResource();
                Cmd.RunClose(_blockWAUFileFullName, string.Empty, waitForExit: true);
            });
        }

        private static void ExtractBlockWAUManifestResource() {
            Type type = typeof(WindowsUtil);
            Assembly assembly = type.Assembly;
            assembly.ExtractManifestResource(type, _blockWAUResourceName, _blockWAUFileFullName);
        }

        public static void Win10Optimize(Action<Exception> callback) {
            Task.Factory.StartNew(() => {
                try {
                    Type type = typeof(WindowsUtil);
                    Assembly assembly = type.Assembly;
                    string name = "Win10Optimize.reg";
                    string fileFullName = Path.Combine(TempPath.TempDirFullName, name);
                    assembly.ExtractManifestResource(type, name, fileFullName);
                    Cmd.RunClose("regedit", $"/s \"{fileFullName}\"", waitForExit: true);
                    callback?.Invoke(null);
                }
                catch (Exception e) {
                    callback?.Invoke(e);
                }
            });
        }
    }
}
