using System;

namespace NTMiner.Core.Kernels {
    public interface IKernelDownloader {
        void Download(Guid kernelId, Action<bool, string> downloadComplete);
    }
}
