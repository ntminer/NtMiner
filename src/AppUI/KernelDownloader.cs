using NTMiner.Core.Kernels;
using NTMiner.Views.Ucs;
using System;

namespace NTMiner {
    public class KernelDownloader : IKernelDownloader {
        public void Download(Guid kernelId, Action<bool, string> downloadComplete) {
            KernelPage.ShowWindow(kernelId, downloadComplete);
        }
    }
}
