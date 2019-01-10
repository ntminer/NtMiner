using System;
using System.IO;
using System.Net;

namespace NTMiner.Core.Kernels.Impl {
    public class PackageDownloader : IPackageDownloader {
        private readonly INTMinerRoot _root;

        public PackageDownloader(INTMinerRoot root) {
            _root = root;
            BootLog.Log(this.GetType().FullName + "接入总线");
        }

        public void Download(
            string package,
            Action<int> progressChanged,
            Action<bool, string, string> downloadComplete,
            out Action cancel) {
            string saveFileFullName = Path.Combine(SpecialPath.DownloadDirFullName, package);
            progressChanged?.Invoke(0);
            using (WebClient webClient = new WebClient()) {
                cancel = () => {
                    webClient.CancelAsync();
                };
                webClient.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => {
                    Execute.OnUIThread(() => {
                        progressChanged?.Invoke(e.ProgressPercentage);
                    });
                };
                webClient.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) => {
                    Execute.OnUIThread(() => {
                        bool isSuccess = !e.Cancelled && e.Error == null;
                        if (isSuccess) {
                            Global.Logger.InfoDebugLine(package + "下载成功");
                        }
                        string message = "下载成功";
                        if (e.Error != null) {
                            message = "下载失败";
                            Global.Logger.ErrorDebugLine(e.Error.Message, e.Error);
                        }
                        if (e.Cancelled) {
                            message = "下载取消";
                        }
                        downloadComplete?.Invoke(isSuccess, message, saveFileFullName);
                    });
                };
                Server.FileUrlService.GetPackageUrl(package, packageUrl => {
                    if (string.IsNullOrEmpty(packageUrl)) {
                        downloadComplete?.Invoke(false, "未获取到内核包下载地址", saveFileFullName);
                    }
                    Global.DebugLine("下载：" + packageUrl);
                    webClient.DownloadFileAsync(new Uri(packageUrl), saveFileFullName);
                });
            }
        }
    }
}
