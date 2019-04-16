using System;
using System.IO;
using System.Reflection;

/// <summary>
/// 注意不要挪动这里的命名空间
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.ThisSystemDir.SysWOW64 {
    public class SysWOW64Util {
        public static void ExtractResource() {
            try {
                Type type = typeof(SysWOW64Util);
                Assembly assembly = type.Assembly;
                string[] fileNames = new string[] {
                    "atiadlxx.dll",
                    "atiadlxy.dll"
                };
                foreach (var name in fileNames) {
                    assembly.ExtractManifestResource(type, name, Path.Combine(SpecialPath.ThisSysWOW64Dir, name));
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
