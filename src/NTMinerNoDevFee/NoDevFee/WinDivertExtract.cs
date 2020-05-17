using System;
using System.IO;
using System.Reflection;

namespace NTMiner.NoDevFee {
    public static class WinDivertExtract {
        private static bool s_extracted = false;
        public static void Extract() {
            if (s_extracted) {
                return;
            }
            s_extracted = true;
            try {
                Type type = typeof(WinDivertExtract);
                Assembly assembly = type.Assembly;
                string[] names = new string[] { "WinDivert.dll", "WinDivert64.sys" };
                foreach (var name in names) {
                    string fileFullName = Path.Combine(HomePath.BaseDirectory, name);
                    Stream stream = assembly.GetManifestResourceStream(type, name);
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    if (File.Exists(fileFullName) && HashUtil.Sha1(data) == HashUtil.Sha1(File.ReadAllBytes(fileFullName))) {
                        continue;
                    }
                    File.WriteAllBytes(fileFullName, data);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
