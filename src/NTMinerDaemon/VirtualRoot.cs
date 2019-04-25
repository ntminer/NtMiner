using System;

namespace NTMiner {
    public class VirtualRoot {
        public static bool IsMinerStudio = false;
        public static string GlobalDirFullName { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
    }
}
