using NTMiner.Core;
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
        public static void BlockWAU() {
            try {
                Task.Factory.StartNew(() => {
                    Type type = typeof(WindowsUtil);
                    Assembly assembly = type.Assembly;
                    string name = "BlockWAU.bat";
                    string fileFullName = Path.Combine(SpecialPath.TempDirFullName, name);
                    assembly.ExtractManifestResource(type, name, fileFullName);
                    Cmd.RunClose(fileFullName, string.Empty, waitForExit: true);
                    VirtualRoot.Happened(new BlockWAUEvent(true, "禁用windows系统更新成功"));
                });
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                VirtualRoot.Happened(new BlockWAUEvent(false, "禁用windows系统更新失败"));
            }
        }

        public static void Win10Optimize() {
            try {
                Task.Factory.StartNew(() => {
                    Type type = typeof(WindowsUtil);
                    Assembly assembly = type.Assembly;
                    string name = "Win10Optimize.reg";
                    string fileFullName = Path.Combine(SpecialPath.TempDirFullName, name);
                    assembly.ExtractManifestResource(type, name, fileFullName);
                    Cmd.RunClose("regedit", $"/s \"{fileFullName}\"", waitForExit: true);
                    VirtualRoot.Happened(new Win10OptimizeEvent(true, "优化Windows成功"));
                });
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                VirtualRoot.Happened(new Win10OptimizeEvent(false, "优化Windows失败"));
            }
        }
    }
}
