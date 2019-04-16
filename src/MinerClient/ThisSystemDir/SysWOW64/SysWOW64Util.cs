using System;
using System.IO;
using System.Reflection;

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
