using NTMiner.Core.Kernels;
using System;
using System.Collections.Generic;
using System.IO;

namespace NTMiner {
    public static class Cleaner {
        // 启动时清理一次即可
        public static void Clear() {
            ClearKernelLogs();
            ClearRootLogs();
            ClearPackages();
            CleanKernels();
            ClearDownload();
        }

        private static bool _clearedDownload = false;
        /// <summary>
        /// 启动时删除Temp/Download目录下的下载文件，启动时调一次即可
        /// </summary>
        private static void ClearDownload() {
            if (_clearedDownload) {
                return;
            }
            _clearedDownload = true;
            try {
                foreach (var file in Directory.GetFiles(SpecialPath.DownloadDirFullName)) {
                    File.Delete(file);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        /// <summary>
        /// 清理掉下载时间超过1个月且服务器已经删除的内核包
        /// </summary>
        private static void ClearPackages() {
            HashSet<string> packageFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kernel in NTMinerRoot.Instance.KernelSet) {
                if (!string.IsNullOrEmpty(kernel.Package)) {
                    packageFileNames.Add(kernel.Package);
                }
            }
            try {
                int n = 0;
                foreach (string file in Directory.GetFiles(SpecialPath.PackagesDirFullName)) {
                    FileInfo fileInfo = new FileInfo(file);
                    bool isPackageExitInServer = packageFileNames.Contains(fileInfo.Name);
                    if (isPackageExitInServer) {
                        continue;
                    }
                    if (fileInfo.LastWriteTime.AddMonths(1) < DateTime.Now) {
                        File.Delete(file);
                        n++;
                    }
                }
                if (n == 0) {
                    Logger.OkDebugLine("没有过期的Package");
                }
                else {
                    Logger.OkDebugLine("过期Package清理完成");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        /// <summary>
        /// 删除除当前正在挖矿的内核外的包解压目录
        /// </summary>
        private static void CleanKernels() {
            try {
                foreach (var kernelProcessName in NTMinerRoot.Instance.KernelSet.GetAllKernelProcessNames()) {
                    if (NTMinerRoot.Instance.CurrentMineContext == null || NTMinerRoot.Instance.CurrentMineContext.Kernel.GetProcessName() != kernelProcessName) {
                        Windows.TaskKill.Kill(kernelProcessName, waitForExit: true);
                    }
                }
                string currentKernelDir = string.Empty;
                var currentMineContext = NTMinerRoot.Instance.CurrentMineContext;
                if (currentMineContext != null && currentMineContext.Kernel != null) {
                    currentKernelDir = currentMineContext.Kernel.GetKernelDirFullName();
                }
                foreach (string dir in Directory.GetDirectories(SpecialPath.KernelsDirFullName)) {
                    if (string.IsNullOrEmpty(currentKernelDir) || dir != currentKernelDir) {
                        try {
                            Directory.Delete(dir, recursive: true);
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e);
                        }
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        /// <summary>
        /// 清理7天前的RootLog
        /// </summary>
        private static void ClearRootLogs() {
            try {
                string logDir = Logging.LogDir.Dir;
                if (string.IsNullOrEmpty(logDir)) {
                    return;
                }
                List<string> toRemoves = new List<string>();
                foreach (var file in Directory.GetFiles(logDir)) {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime.AddDays(7) < DateTime.Now) {
                        toRemoves.Add(file);
                    }
                }
                if (toRemoves.Count == 0) {
                    Logger.OkDebugLine("没有过期的RootLog");
                }
                else {
                    foreach (var item in toRemoves) {
                        File.Delete(item);
                    }
                    Logger.OkDebugLine("过期RootLog清理完成");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        /// <summary>
        /// 清理7天前的内核日志
        /// </summary>
        private static void ClearKernelLogs() {
            try {
                List<string> toRemoves = new List<string>();
                foreach (var file in Directory.GetFiles(SpecialPath.LogsDirFullName)) {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime.AddDays(7) < DateTime.Now) {
                        toRemoves.Add(file);
                    }
                }
                if (toRemoves.Count == 0) {
                    Logger.OkDebugLine("没有过期的KernelLog");
                }
                else {
                    foreach (var item in toRemoves) {
                        File.Delete(item);
                    }
                    Logger.OkDebugLine("过期KernelLog清理完成");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
