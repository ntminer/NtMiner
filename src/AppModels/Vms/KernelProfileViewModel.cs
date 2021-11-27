using NTMiner.Core.Kernels;
using NTMiner.Core.Profiles;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelProfileViewModel : ViewModelBase, IKernelProfile {
        public static readonly KernelProfileViewModel Empty = new KernelProfileViewModel(KernelViewModel.Empty, NTMinerContext.Instance.KernelProfileSet?.EmptyKernelProfile/*设计视图才可能为null*/) {
            _cancelDownload = null,
            _downloadMessage = string.Empty,
            _downloadPercent = 0,
            _isDownloading = false,
        };

        private string _downloadMessage;
        private bool _isDownloading = false;
        private double _downloadPercent;
        private string _installText = "安装";
        private string _unInstallText = "卸载";

        private Action _cancelDownload;

        private readonly IKernelProfile _kernelProfile;
        public ICommand CancelDownload { get; private set; }
        public ICommand Install { get; private set; }
        public ICommand UnInstall { get; private set; }

        private readonly KernelViewModel _kernelVm;
        public KernelProfileViewModel(KernelViewModel kernelVm, IKernelProfile kernelProfile) {
            _kernelVm = kernelVm;
            _kernelProfile = kernelProfile;
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.CancelDownload = new DelegateCommand(() => {
                _cancelDownload?.Invoke();
            });
            this.Install = new DelegateCommand(() => {
                if (!ClientAppType.IsMinerClient) {
                    VirtualRoot.Out.ShowWarn("非挖矿端不需要安装内核", autoHideSeconds: 4);
                    return;
                }
                this.Download();
            });
            this.UnInstall = new DelegateCommand(() => {
                if (this.UnInstallText == "确认卸载") {
                    string processName = _kernelVm.GetProcessName();
                    if (NTMinerContext.Instance.IsMining) {
                        if (NTMinerContext.Instance.LockedMineContext.Kernel.Package == _kernelVm.Package) {
                            VirtualRoot.Out.ShowWarn("该内核正在挖矿，请停止挖矿后再卸载", autoHideSeconds: 4);
                            return;
                        }
                    }
                    string packageFileFullName = _kernelVm.GetPackageFileFullName();
                    if (!string.IsNullOrEmpty(packageFileFullName)) {
                        File.Delete(packageFileFullName);
                    }
                    string kernelDirFullName = _kernelVm.GetKernelDirFullName();
                    if (!string.IsNullOrEmpty(kernelDirFullName) && Directory.Exists(kernelDirFullName)) {
                        try {
                            Directory.Delete(kernelDirFullName, recursive: true);
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e);
                        }
                    }
                    string downloadFileFullName = _kernelVm.GetDownloadFileFullName();
                    if (!string.IsNullOrEmpty(downloadFileFullName)) {
                        File.Delete(downloadFileFullName);
                    }
                    Refresh();
                    this.InstallText = "卸载成功";
                    2.SecondsDelay().ContinueWith(t => {
                        this.InstallText = "安装";
                    });
                }
                else {
                    this.UnInstallText = "确认卸载";
                    2.SecondsDelay().ContinueWith(t => {
                        this.UnInstallText = "卸载";
                    });
                }
            });
        }

        public void Refresh() {
            OnPropertyChanged(nameof(InstallStatus));
            OnPropertyChanged(nameof(InstallStatusDescription));
            OnPropertyChanged(nameof(BtnInstallVisible));
            OnPropertyChanged(nameof(BtnInstalledVisible));
        }

        public Guid KernelId {
            get => _kernelProfile.KernelId;
        }

        public InstallStatus InstallStatus {
            get => _kernelProfile.InstallStatus;
        }

        public string InstallStatusDescription {
            get {
                return InstallStatus.GetDescription();
            }
        }

        public Visibility BtnInstallVisible {
            get {
                if (InstallStatus == InstallStatus.Uninstalled) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility BtnInstalledVisible {
            get {
                if (InstallStatus == InstallStatus.Installed) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public bool IsDownloading {
            get { return _isDownloading; }
            set {
                if (_isDownloading != value) {
                    _isDownloading = value;
                    OnPropertyChanged(nameof(IsDownloading));
                    Refresh();
                    AppRoot.KernelVms.OnIsDownloadingChanged(_kernelVm);
                }
            }
        }

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

        public string UnInstallText {
            get { return _unInstallText; }
            set {
                if (_unInstallText != value) {
                    _unInstallText = value;
                    OnPropertyChanged(nameof(UnInstallText));
                }
            }
        }

        public string InstallText {
            get { return _installText; }
            set {
                if (_installText != value) {
                    _installText = value;
                    OnPropertyChanged(nameof(InstallText));
                }
            }
        }

        #region Download
        public void Download(Action<bool, string> downloadComplete = null) {
            if (this.IsDownloading) {
                return;
            }
            this.IsDownloading = true;
            var otherSamePackageKernelVms = AppRoot.KernelVms.AllKernels.Where(a => a.Package == this._kernelVm.Package && a != this._kernelVm).ToList();
            foreach (var kernelVm in otherSamePackageKernelVms) {
                kernelVm.KernelProfileVm.IsDownloading = true;
            }
            string package = _kernelVm.Package;
            Download(package, progressChanged: (percent) => {
                this.DownloadMessage = percent + "%";
                this.DownloadPercent = (double)percent / 100;
            }, downloadComplete: (isSuccess, message, saveFileFullName) => {
                this.DownloadMessage = message;
                this.DownloadPercent = 0;
                if (isSuccess) {
                    if (!Directory.Exists(HomePath.PackagesDirFullName)) {
                        Directory.CreateDirectory(HomePath.PackagesDirFullName);
                    }
                    File.Copy(saveFileFullName, Path.Combine(HomePath.PackagesDirFullName, package), overwrite: true);
                    File.Delete(saveFileFullName);
                    this.IsDownloading = false;
                    foreach (var kernelVm in otherSamePackageKernelVms) {
                        kernelVm.KernelProfileVm.IsDownloading = false;
                    }
                    this.UnInstallText = "安装成功";
                    2.SecondsDelay().ContinueWith(t => {
                        this.UnInstallText = "卸载";
                    });
                }
                else {
                    2.SecondsDelay().ContinueWith((t) => {
                        this.IsDownloading = false;
                        foreach (var kernelVm in otherSamePackageKernelVms) {
                            kernelVm.KernelProfileVm.IsDownloading = false;
                        }
                    });
                }
                downloadComplete?.Invoke(isSuccess, message);
            }, cancel: out _cancelDownload);
        }

        private static void Download(
            string package,
            Action<int> progressChanged,
            // isSuccess, message, saveFileFullName
            Action<bool, string, string> downloadComplete,
            out Action cancel) {
            string saveFileFullName = Path.Combine(MinerClientTempPath.DownloadDirFullName, package);
            progressChanged?.Invoke(0);
            using (var webClient = VirtualRoot.CreateWebClient()) {
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
                            VirtualRoot.ThisLocalInfo(nameof(KernelProfileViewModel), package + "下载成功", toConsole: true);
                        }
                        string message = "下载成功";
                        if (e.Error != null) {
                            message = "下载失败，请检查网络";
                            string errorMessage = e.Error.GetInnerMessage();
                            VirtualRoot.Out.ShowError(errorMessage);
                            // 这里就不记录异常了，因为异常很可能是因为磁盘空间不足
                        }
                        if (e.Cancelled) {
                            message = "已取消";
                        }
                        downloadComplete?.Invoke(isSuccess, message, saveFileFullName);
                    });
                };
                RpcRoot.OfficialServer.FileUrlService.GetPackageUrlAsync(package, (packageUrl, e) => {
                    if (e != null && e is NTMinerHttpException httpE) {
                        if (httpE.StatusCode >= HttpStatusCode.InternalServerError) {
                            VirtualRoot.Out.ShowError("服务器忙");
                        }
                    }
                    else {
                        if (string.IsNullOrEmpty(packageUrl)) {
                            string msg = $"未获取到{package}内核包下载地址";
                            downloadComplete?.Invoke(false, msg, saveFileFullName);
                            VirtualRoot.ThisLocalError(nameof(KernelProfileViewModel), msg, toConsole: true);
                        }
                        else {
                            VirtualRoot.ThisLocalInfo(nameof(KernelProfileViewModel), "下载：" + package, toConsole: true);
                            webClient.DownloadFileAsync(new Uri(packageUrl), saveFileFullName);
                        }
                    }
                });
            }
        }
        #endregion
    }
}
