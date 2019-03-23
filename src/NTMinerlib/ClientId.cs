using System;
using System.Diagnostics;
using System.IO;

namespace NTMiner {
    public static class ClientId {
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public static Guid Id { get; private set; }

        public static string LangDbFileFullName { get; private set; }
        public static string LocalLangJsonFileFullName { get; private set; }

        public static string ReadLocalLangJsonFile() {
            if (File.Exists(LocalLangJsonFileFullName)) {
                return File.ReadAllText(LocalLangJsonFileFullName);
            }

            return string.Empty;
        }

        public static void WriteLocalLangJsonFile(string json) {
            File.WriteAllText(LocalLangJsonFileFullName, json);
        }

        static ClientId() {
            if (!Directory.Exists(VirtualRoot.GlobalDirFullName)) {
                Directory.CreateDirectory(VirtualRoot.GlobalDirFullName);
            }
            LangDbFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "lang.litedb");
            LocalLangJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "lang.json");
            Id = NTMinerRegistry.GetClientId();
        }
    }
}
