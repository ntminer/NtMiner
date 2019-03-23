using System;
using System.IO;

namespace NTMiner {
    public static class DropOld {
        /// <summary>
        /// 如果当前启动的升级器不是AppData/Local/NTMiner/Updater/NTMinerUpdater.exe位置的升级器
        /// </summary>
        public static void DropOldUpdater() {
            if (!AppDomain.CurrentDomain.BaseDirectory.StartsWith(VirtualRoot.GlobalDirFullName)) {
                try {
                    File.Delete(Path.Combine(VirtualRoot.GlobalDirFullName, "Updater", "NTMinerUpdater.exe"));
                }
                catch {
                }
            }
        }
    }
}
