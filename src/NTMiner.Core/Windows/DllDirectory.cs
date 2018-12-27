using System.Runtime.InteropServices;

namespace NTMiner.Windows {
    public static class DllDirectory {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDllDirectory(string lpPathName);
    }
}
