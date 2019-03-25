using System;
using System.IO;
using System.Net;

namespace NTMiner.Core.Kernels.Impl {
    public class PackageDownloader : IPackageDownloader {
        private readonly INTMinerRoot _root;

        public PackageDownloader(INTMinerRoot root) {
            _root = root;
        }

        public void Download(
            string package,
            Action<int> progressChanged,
            // isSuccess, message, saveFileFullName
            Action<bool, string, string> downloadComplete,
            out Action cancel) {
            string saveFileFullName = Path.Combine(SpecialPath.DownloadDirFullName, package);
            progressChanged?.Invoke(0);
            using (WebClient webClient = new WebClient()) {
                cancel = () => {
                    webClient.CancelAsync();
                };
                webClient.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => {
                    UIThread.Execute(() => {
                        progressChanged?.Invoke(e.ProgressPercentage);
                    });
                };
                webClient.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) => {
                    UIThread.Execute(() => {
                        bool isSuccess = !e.Cancelled && e.Error == null;
                        if (isSuccess) {
                            Logger.OkDebugLine(package + "下载成功");
                        }
                        string message = "下载成功";
                        if (e.Error != null) {
                            message = "下载失败";
                            Logger.ErrorDebugLine(e.Error.Message, e.Error);
                        }
                        if (e.Cancelled) {
                            message = "下载取消";
                        }
                        downloadComplete?.Invoke(isSuccess, message, saveFileFullName);
                    });
                };
                OfficialServer.FileUrlService.GetPackageUrlAsync(package, (packageUrl, e) => {
                    if (string.IsNullOrEmpty(packageUrl)) {
                        downloadComplete?.Invoke(false, "未获取到内核包下载地址", saveFileFullName);
                    }
                    Logger.InfoDebugLine("下载：" + packageUrl);
                    webClient.DownloadFileAsync(new Uri(packageUrl), saveFileFullName);
                });
            }
        }
    }
}
