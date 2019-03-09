using System;
using System.Diagnostics;
using System.IO;

namespace NTMiner {
    public static class ClientId {        
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public static Guid Id { get; private set; }

        public static string LangDbFileName { get; private set; }
        public static string LangDbFileFullName { get; private set; }
        public static string LocalLangJsonFileName { get; private set; }
        public static string LocalLangJsonFileFullName { get; private set; }

        static ClientId() {
            if (!Directory.Exists(VirtualRoot.GlobalDirFullName)) {
                Directory.CreateDirectory(VirtualRoot.GlobalDirFullName);
            }
            LangDbFileName = "lang.litedb";
            LangDbFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, LangDbFileName);
            LocalLangJsonFileName = "lang.json";
            LocalLangJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, LocalLangJsonFileName);
            Id = NtMinerRegistry.GetClientId();
        }
    }
}
