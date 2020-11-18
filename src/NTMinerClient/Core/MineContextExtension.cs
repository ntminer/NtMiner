using NTMiner.Core.Kernels;
using NTMiner.Mine;
using System;
using System.IO;

namespace NTMiner.Core {
    public static class MineContextExtension {
        public static void ExecuteFileWriters(this IMineContext mineContext) {
            try {
                // 执行文件书写器
                foreach (var fileWriterId in mineContext.CoinKernel.FileWriterIds) {
                    if (NTMinerContext.Instance.ServerContext.FileWriterSet.TryGetFileWriter(fileWriterId, out IFileWriter fileWriter)) {
                        Execute(mineContext, fileWriter);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static void Execute(IMineContext mineContext, IFileWriter writer) {
            try {
                string content = string.Empty;
                mineContext.FileWriters.TryGetValue(writer.GetId(), out content);
                if (!string.IsNullOrEmpty(content)) {
                    // 因为内核日志文件名不是提前确定的而是创建进程前确定的
                    content = content.Replace(NTKeyword.LogFileParameterName, mineContext.LogFileFullName);
                    string fileFullName = Path.Combine(mineContext.Kernel.GetKernelDirFullName(), writer.FileUrl);
                    File.WriteAllText(fileFullName, content);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
