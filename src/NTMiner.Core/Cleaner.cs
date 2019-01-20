using NTMiner.Core.Kernels;
using System;
using System.Collections.Generic;
using System.IO;

namespace NTMiner {
    public static class Cleaner {
        public static void ClearPackages() {

        }

        public static void CleanKernels() {
            try {
                string currentKernelDir = string.Empty;
                var currentMineContext = NTMinerRoot.Current.CurrentMineContext;
                if (currentMineContext != null && currentMineContext.Kernel != null) {
                    currentKernelDir = currentMineContext.Kernel.GetKernelDirFullName();
                }
                foreach (string dir in Directory.GetDirectories(SpecialPath.KernelsDirFullName)) {
                    if (string.IsNullOrEmpty(currentKernelDir) || dir != currentKernelDir) {
                        try {
                            Directory.Delete(dir, recursive: true);
                        }
                        catch (Exception e) {
                            Global.Logger.ErrorDebugLine(e.Message, e);
                        }
                    }
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        public static void ClearRootLogs() {
            try {
                string logDir = Logging.LogDir.Dir;
                if (string.IsNullOrEmpty(logDir)) {
                    return;
                }
                List<string> toRemoves = new List<string>();
                foreach (var file in Directory.GetFiles(logDir)) {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime.AddDays(2) < DateTime.Now) {
                        toRemoves.Add(file);
                    }
                }
                if (toRemoves.Count == 0) {
                    Global.Logger.OkDebugLine("没有过期的RootLog");
                }
                else {
                    foreach (var item in toRemoves) {
                        File.Delete(item);
                    }
                    Global.Logger.OkDebugLine("过期RootLog清理完成");
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        public static void ClearKernelLogs() {
            try {
                List<string> toRemoves = new List<string>();
                foreach (var file in Directory.GetFiles(SpecialPath.LogsDirFullName)) {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime.AddDays(7) < DateTime.Now) {
                        toRemoves.Add(file);
                    }
                }
                if (toRemoves.Count == 0) {
                    Global.Logger.OkDebugLine("没有过期的KernelLog");
                }
                else {
                    foreach (var item in toRemoves) {
                        File.Delete(item);
                    }
                    Global.Logger.OkDebugLine("过期KernelLog清理完成");
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
