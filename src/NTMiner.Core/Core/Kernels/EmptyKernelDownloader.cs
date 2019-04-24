using System;

namespace NTMiner.Core.Kernels {
    public class EmptyKernelDownloader : IKernelDownloader {
        public void Download(Guid kernelId, Action<bool, string> downloadComplete) {
            downloadComplete?.Invoke(false, string.Empty);
        }
    }
}
