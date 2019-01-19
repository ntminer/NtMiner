using System.Diagnostics;

namespace NTMiner {
    public static class ClientId {
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public const string NTMinerRegistrySubKey = @".DEFAULT\Software\NTMiner";
    }
}
