using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.AtikmdagPatcher {
    public static class AtikmdagPatcherUtil {
        public static void Run() {
            try {
                Task.Factory.StartNew(() => {
                    Type type = typeof(AtikmdagPatcherUtil);
                    Assembly assembly = type.Assembly;
                    string name = "atikmdag-patcher1.4.6.exe";
                    string fileFullName = Path.Combine(SpecialPath.TempDirFullName, name);
                    assembly.ExtractManifestResource(type, name, fileFullName);
                    Windows.Cmd.RunClose(fileFullName, string.Empty, waitForExit: false);
                });
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
