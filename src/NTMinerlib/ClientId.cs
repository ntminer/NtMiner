using System;
using System.Diagnostics;
using System.IO;

namespace NTMiner {
    public static class ClientId {        
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public static Guid Id { get; private set; }

        public static string GlobalDirFullName { get; private set; }
        public static string LangDbFileName { get; private set; }
        public static string LangDbFileFullName { get; private set; }
        public static string LocalLangJsonFileName { get; private set; }
        public static string LocalLangJsonFileFullName { get; private set; }

        static ClientId() {
            GlobalDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
            if (!Directory.Exists(GlobalDirFullName)) {
                Directory.CreateDirectory(GlobalDirFullName);
            }
            LangDbFileName = "lang.litedb";
            LangDbFileFullName = Path.Combine(GlobalDirFullName, LangDbFileName);
            LocalLangJsonFileName = "lang.json";
            LocalLangJsonFileFullName = Path.Combine(GlobalDirFullName, LocalLangJsonFileName);
            Id = NTMinerRegistry.GetClientId();
        }
    }
}
