using System;
using System.Diagnostics;

namespace NTMiner {
    public class VirtualRoot {
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public static bool IsMinerStudio = false;
        public static string LocalDirFullName { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
    }
}
