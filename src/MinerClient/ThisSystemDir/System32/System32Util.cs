using System;
using System.IO;
using System.Reflection;

/// <summary>
/// 注意不要挪动这里的命名空间
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.ThisSystemDir.System32 {
    public class System32Util {
        public static void ExtractResource() {
            try {
                Type type = typeof(System32Util);
                Assembly assembly = type.Assembly;
                string[] fileNames = new string[] {
                    "atiadlxx.dll"
                };
                foreach (var name in fileNames) {
                    assembly.ExtractManifestResource(type, name, Path.Combine(SpecialPath.ThisSystem32Dir, name));
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
