using System.Runtime.InteropServices;

namespace NTMiner.Windows {
    internal static partial class NativeMethods {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDllDirectory(string lpPathName);
    }
}
