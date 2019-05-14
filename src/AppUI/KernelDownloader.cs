using NTMiner.Core.Kernels;
using NTMiner.Views;
using System;

namespace NTMiner {
    public class KernelDownloader : IKernelDownloader {
        public KernelDownloader() {
        }

        public void Download(Guid kernelId, Action<bool, string> downloadComplete) {
            UIThread.Execute(() => {
                KernelsWindow.ShowWindow(kernelId, downloadComplete);
            });
        }
    }
}
