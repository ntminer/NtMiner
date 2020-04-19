using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class FileDownloaderViewModel : ViewModelBase {
        private readonly string _downloadFileUrl;
        private string _downloadMessage;
        private Action _cancel;
        private Visibility _btnCancelVisible = Visibility.Visible;

        public ICommand CancelDownload { get; private set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public FileDownloaderViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public FileDownloaderViewModel(string downloadFileUrl, Action<bool, string, string> downloadComplete) {
            if (string.IsNullOrEmpty(downloadFileUrl)) {
                throw new InvalidProgramException();
            }
            _downloadFileUrl = downloadFileUrl;
            this.CancelDownload = new DelegateCommand(() => {
                this._cancel?.Invoke();
            });
            this.Download(downloadComplete);
        }

        public string DownloadFileUrl {
            get {
                return _downloadFileUrl;
            }
        }

        private double _downloadPercent;
        public double DownloadPercent {
            get {
                return _downloadPercent;
            }
            set {
                if (_downloadPercent != value) {
                    _downloadPercent = value;
                    OnPropertyChanged(nameof(DownloadPercent));
                }
            }
        }

        public string DownloadMessage {
            get {
                return _downloadMessage;
            }
            set {
                if (_downloadMessage != value) {
                    _downloadMessage = value;
                    OnPropertyChanged(nameof(DownloadMessage));
                }
            }
        }

        public Visibility BtnCancelVisible {
            get => _btnCancelVisible;
            set {
                if (_btnCancelVisible != value) {
                    _btnCancelVisible = value;
                    OnPropertyChanged(nameof(BtnCancelVisible));
                }
            }
        }

        private void Download(Action<bool, string, string> downloadComplete) {
            Logger.InfoDebugLine("下载：" + _downloadFileUrl);
            string saveFileFullName = Path.Combine(TempPath.DownloadDirFullName, Guid.NewGuid().ToString());
            using (var webClient = VirtualRoot.CreateWebClient()) {
                _cancel = () => {
                    webClient.CancelAsync();
                };
                webClient.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => {
                    this.DownloadMessage = e.ProgressPercentage + "%";
                    this.DownloadPercent = (double)e.ProgressPercentage / 100;
                };
                webClient.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) => {
                    bool isSuccess = !e.Cancelled && e.Error == null;
                    if (isSuccess) {
                        Logger.OkDebugLine($"{_downloadFileUrl}下载成功");
                    }
                    string message = "下载成功";
                    if (e.Error != null) {
                        message = "下载失败，请检查网络";
                        Logger.ErrorDebugLine(e.Error.Message, e.Error);
                    }
                    if (e.Cancelled) {
                        message = "已取消";
                    }
                    this.DownloadMessage = message;
                    this.DownloadPercent = 0;
                    this.BtnCancelVisible = Visibility.Collapsed;
                    2.SecondsDelay().ContinueWith((t) => {
                        downloadComplete?.Invoke(isSuccess, message, saveFileFullName);
                    });
                };
                webClient.DownloadFileAsync(
                    new Uri(_downloadFileUrl),
                    saveFileFullName);
            }
        }
    }
}
