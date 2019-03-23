using NTMiner.Core.Kernels;
using NTMiner.Core.Profiles;
using NTMiner.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelProfileViewModel : ViewModelBase, IKernelProfile {
        public static readonly KernelProfileViewModel Empty = new KernelProfileViewModel(KernelViewModel.Empty, NTMinerRoot.Current.KernelProfileSet.EmptyKernelProfile) {
            _cancelDownload = null,
            _downloadMessage = string.Empty,
            _downloadPercent = 0,
            _isDownloading = false,
        };

        private string _downloadMessage;
        private bool _isDownloading = false;
        private double _downloadPercent;

        private Action _cancelDownload;

        private readonly IKernelProfile _kernelProfile;
        public ICommand CancelDownload { get; private set; }
        public ICommand Install { get; private set; }
        public ICommand UnInstall { get; private set; }

        private KernelViewModel _kernelVm;
        public KernelProfileViewModel(KernelViewModel kernelVm, IKernelProfile kernelProfile) {
            _kernelVm = kernelVm;
            _kernelProfile = kernelProfile;
            this.CancelDownload = new DelegateCommand(() => {
                _cancelDownload?.Invoke();
            });
            this.Install = new DelegateCommand(() => {
                this.Download();
            });
            this.UnInstall = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定卸载{_kernelVm.FullName}内核吗？", title: "确认", onYes: () => {
                    string processName = _kernelVm.GetProcessName();
                    if (!string.IsNullOrEmpty(processName)) {
                        Windows.TaskKill.Kill(processName);
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
                                Logger.ErrorDebugLine(e.Message, e);
                            }
                        }
                        string downloadFileFullName = _kernelVm.GetDownloadFileFullName();
                        if (!string.IsNullOrEmpty(downloadFileFullName)) {
                            File.Delete(downloadFileFullName);
                        }
                    }
                    Refresh();
                    KernelPageViewModel.Current.OnPropertyChanged(nameof(KernelPageViewModel.QueryResults));
                }, icon: IconConst.IconConfirm);
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
                    KernelPageViewModel.Current.OnPropertyChanged(nameof(KernelPageViewModel.DownloadingVms));
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

        #region Download
        public void Download(Action<bool, string> downloadComplete = null) {
            if (this.IsDownloading) {
                return;
            }
            this.IsDownloading = true;
            var otherSamePackageKernelVms = KernelViewModels.Current.AllKernels.Where(a => a.Package == this._kernelVm.Package && a != this._kernelVm).ToList();
            foreach (var kernelVm in otherSamePackageKernelVms) {
                kernelVm.KernelProfileVm.IsDownloading = true;
            }
            string package = _kernelVm.Package;
            NTMinerRoot.Current.PackageDownloader.Download(package, progressChanged: (percent) => {
                this.DownloadMessage = percent + "%";
                this.DownloadPercent = (double)percent / 100;
            }, downloadComplete: (isSuccess, message, saveFileFullName) => {
                this.DownloadMessage = message;
                this.DownloadPercent = 0;
                if (isSuccess) {
                    File.Copy(saveFileFullName, Path.Combine(SpecialPath.PackagesDirFullName, package), overwrite: true);
                    File.Delete(saveFileFullName);
                    this.IsDownloading = false;
                    foreach (var kernelVm in otherSamePackageKernelVms) {
                        kernelVm.KernelProfileVm.IsDownloading = false;
                    }
                }
                else {
                    TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                        this.IsDownloading = false;
                        foreach (var kernelVm in otherSamePackageKernelVms) {
                            kernelVm.KernelProfileVm.IsDownloading = false;
                        }
                    });
                }
                downloadComplete?.Invoke(isSuccess, message);
            }, cancel: out _cancelDownload);
        }
        #endregion
    }
}
