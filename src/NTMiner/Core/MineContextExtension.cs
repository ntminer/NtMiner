using NTMiner.Core.Kernels;
using System;
using System.IO;

namespace NTMiner.Core {
    public static class MineContextExtension {
        public static void ExecuteFileWriters(this IMineContext mineContext) {
            try {
                // 执行文件书写器
                foreach (var fileWriterId in mineContext.CoinKernel.FileWriterIds) {
                    if (NTMinerRoot.Instance.ServerContext.FileWriterSet.TryGetFileWriter(fileWriterId, out IFileWriter fileWriter)) {
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
