using System;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.AtikmdagPatcher {
    public static class AtikmdagPatcherUtil {
        public static Task Run() {
            return Task.Factory.StartNew(() => {
                DoRun();
            });
        }

        public static void DoRun() {
            ExtractManifestResource();
            Windows.Cmd.RunClose(App.AtikmdagPatcherFileFullName, string.Empty, waitForExit: false);
        }

        private static void ExtractManifestResource() {
            Type type = typeof(AtikmdagPatcherUtil);
            Assembly assembly = type.Assembly;
            assembly.ExtractManifestResource(type, App.AtikmdagPatcherResourceName, App.AtikmdagPatcherFileFullName);
        }
    }
}
