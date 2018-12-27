using System;

namespace NTMiner.Core.Kernels {
    public interface IPackageDownloader {
        void Download(
            string package,
            Action<int> progressChanged, 
            Action<bool, string, string> downloadComplete,
            out Action cancel);
    }
}
