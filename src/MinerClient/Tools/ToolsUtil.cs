using System;
using System.IO;
using System.Reflection;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.Tools {
    public static class ToolsUtil {
        public static void RegCmdHere() {
            try {
                Type type = typeof(ToolsUtil);
                Assembly assembly = type.Assembly;
                string name = "CmdHere.reg";
                string fileFullName = Path.Combine(SpecialPath.TempDirFullName, name);
                assembly.ExtractManifestResource(type, name, fileFullName);
                Windows.Cmd.RunClose(fileFullName, string.Empty);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
