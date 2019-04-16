using System;
using System.IO;
using System.Reflection;

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
