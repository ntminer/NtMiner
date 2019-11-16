using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.Splash {
    public static class NTMinerSplash {
        public static void Run() {
            try {
                Task.Factory.StartNew(() => {
                    Type type = typeof(NTMinerSplash);
                    Assembly assembly = type.Assembly;
                    string name = "NTMinerSplash.exe";
                    string fileFullName = Path.Combine(MainAssemblyInfo.TempDirFullName, name);
                    assembly.ExtractManifestResource(type, name, fileFullName);
                    Windows.Cmd.RunClose(fileFullName, $"VersionFullName={AppStatic.VersionFullName} ParentProcessId={Process.GetCurrentProcess().Id}", waitForExit: false);
                });
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public static void Kill() {
            Windows.TaskKill.Kill("NTMinerSplash.exe");
        }
    }
}