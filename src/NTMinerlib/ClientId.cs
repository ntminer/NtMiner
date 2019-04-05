using System;
using System.Diagnostics;
using System.IO;

namespace NTMiner {
    public static class ClientId {
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public static Guid Id { get; private set; }

        static ClientId() {
            if (!Directory.Exists(VirtualRoot.GlobalDirFullName)) {
                Directory.CreateDirectory(VirtualRoot.GlobalDirFullName);
            }
            Id = NTMinerRegistry.GetClientId();
        }
    }
}
