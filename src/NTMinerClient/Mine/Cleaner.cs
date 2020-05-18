using NTMiner.Core.Kernels;
using System;
using System.Collections.Generic;
using System.IO;

namespace NTMiner.Mine {
    public class Cleaner {
        public static Cleaner Instance { get; private set; } = new Cleaner();

        private Cleaner() { }

        public void Clear() {
            ClearLogs();
            ClearPackages();
            CleanKernels();
            ClearDownload();
        }

        /// <summary>
        /// 启动时删除Temp/Download目录下的下载文件，启动时调一次即可
        /// </summary>
        private void ClearDownload() {
            try {
                foreach (var file in Directory.GetFiles(MinerClientTempPath.DownloadDirFullName)) {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime.AddDays(1) < DateTime.Now) {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        /// <summary>
        /// 清理掉下载时间超过1个月且服务器已经删除的内核包
        /// </summary>
        private void ClearPackages() {
            HashSet<string> packageFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kernel in NTMinerContext.Instance.ServerContext.KernelSet.AsEnumerable()) {
                if (!string.IsNullOrEmpty(kernel.Package)) {
                    packageFileNames.Add(kernel.Package);
                }
            }
            try {
                int n = 0;
                foreach (string file in Directory.GetFiles(HomePath.PackagesDirFullName)) {
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
        private void CleanKernels() {
            try {
                foreach (var kernelProcessName in NTMinerContext.Instance.ServerContext.KernelSet.GetAllKernelProcessNames()) {
                    if (!NTMinerContext.Instance.IsMining || NTMinerContext.Instance.LockedMineContext.Kernel.GetProcessName() != kernelProcessName) {
                        Windows.TaskKill.Kill(kernelProcessName, waitForExit: true);
                    }
                }
                string currentKernelDir = string.Empty;
                var currentMineContext = NTMinerContext.Instance.LockedMineContext;
                if (currentMineContext != null && currentMineContext.Kernel != null) {
                    currentKernelDir = currentMineContext.Kernel.GetKernelDirFullName();
                }
                foreach (string dir in Directory.GetDirectories(MinerClientTempPath.KernelsDirFullName)) {
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

        private void ClearLogs() {
            try {
                List<string> toRemoves = new List<string>();
                foreach (var file in Directory.GetFiles(MinerClientTempPath.TempLogsDirFullName)) {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime.AddDays(7) < DateTime.Now) {
                        toRemoves.Add(file);
                    }
                }
                if (toRemoves.Count == 0) {
                    Logger.OkDebugLine("没有过期的Log");
                }
                else {
                    foreach (var item in toRemoves) {
                        File.Delete(item);
                    }
                    Logger.OkDebugLine("过期Log清理完成");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
