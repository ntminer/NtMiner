using NTMiner.MinerServer;
using NTMiner.Views;
using NTMiner.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        public static readonly MainWindowViewModel Current = new MainWindowViewModel();

        private double _downloadPercent;
        private bool _isDownloading = false;
        private NTMinerFileViewModel _selectedNTMinerFile;
        private NTMinerFileViewModel _serverLatestVm;
        private bool _isReady;
        private bool _localIsLatest;

        private List<NTMinerFileViewModel> _nTMinerFiles;
        private Visibility _isHistoryVisible = Visibility.Collapsed;

        private string _downloadMessage;
        private Visibility _btnCancelVisible = Visibility.Visible;

        private Action cancel;
        public ICommand Install { get; private set; }
        public ICommand CancelDownload { get; private set; }
        public ICommand ShowHistory { get; private set; }
        public ICommand AddNTMinerFile { get; private set; }

        private MainWindowViewModel() {
            if (App.IsInDesignMode) {
                return;
            }
            this.Refresh();
            this.CancelDownload = new DelegateCommand(() => {
                this.cancel?.Invoke();
                this.IsDownloading = false;
            });
            this.Install = new DelegateCommand(() => {
                if (this.IsDownloading) {
                    return;
                }
                this.IsDownloading = true;
                string ntMinerFile = string.Empty;
                string version = string.Empty;
                if (IsHistoryVisible == Visibility.Collapsed) {
                    if (ServerLatestVm != null) {
                        ntMinerFile = ServerLatestVm.FileName;
                        version = ServerLatestVm.Version;
                    }
                }
                else {
                    ntMinerFile = SelectedNTMinerFile.FileName;
                    version = SelectedNTMinerFile.Version;
                }
                Download(ntMinerFile, version, progressChanged: (percent) => {
                    this.DownloadMessage = percent + "%";
                    this.DownloadPercent = (double)percent / 100;
                }, downloadComplete: (isSuccess, message, saveFileFullName) => {
                    this.DownloadMessage = message;
                    this.DownloadPercent = 0;
                    this.BtnCancelVisible = Visibility.Collapsed;
                    if (isSuccess) {
                        this.DownloadMessage = "更新成功，正在重启";
                        if (VirtualRoot.IsMinerStudio) {
                            Client.MinerStudioService.CloseMinerStudio();
                        }
                        else {
                            Client.MinerClientService.CloseNTMiner();
                        }
                        TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                            string location = NTMinerRegistry.GetLocation();
                            if (string.IsNullOrEmpty(location) || !File.Exists(location)) {
                                location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ntMinerFile);
                            }
                            File.Copy(saveFileFullName, location, overwrite: true);
                            File.Delete(saveFileFullName);
                            string arguments = NTMinerRegistry.GetArguments();
                            Process.Start(location, arguments);
                            this.IsDownloading = false;
                            UIThread.Execute(() => {
                                Application.Current.MainWindow.Close();
                            });
                        });
                    }
                    else {
                        TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                            this.IsDownloading = false;
                        });
                    }
                }, cancel: out cancel);
            });
            this.ShowHistory = new DelegateCommand(() => {
                if (IsHistoryVisible == Visibility.Visible) {
                    IsHistoryVisible = Visibility.Collapsed;
                }
                else {
                    IsHistoryVisible = Visibility.Visible;
                }
            });
            this.AddNTMinerFile = new DelegateCommand(() => {
                NTMinerFileEdit window = new NTMinerFileEdit("添加", "Icon_Add", new NTMinerFileViewModel() {
                    AppType = App.AppType
                });
                window.ShowDialogEx();
            });
        }

        public static void Download(
            string fileName,
            string version,
            Action<int> progressChanged,
            Action<bool, string, string> downloadComplete,
            out Action cancel) {
            Logger.InfoDebugLine("下载：" + fileName);
            string saveFileFullName = Path.Combine(SpecialPath.DownloadDirFullName, App.AppType.ToString() + version);
            progressChanged?.Invoke(0);
            using (WebClient webClient = new WebClient()) {
                cancel = () => {
                    webClient.CancelAsync();
                };
                webClient.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => {
                    progressChanged?.Invoke(e.ProgressPercentage);
                };
                webClient.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) => {
                    bool isSuccess = !e.Cancelled && e.Error == null;
                    string message = "下载成功";
                    if (e.Error != null) {
                        message = "下载失败";
                        Logger.ErrorDebugLine(e.Error.Message, e.Error);
                    }
                    if (e.Cancelled) {
                        message = "下载取消";
                    }
                    if (isSuccess) {
                        NotiCenterWindowViewModel.Current.Manager.ShowSuccessMessage(App.AppType.ToString() + version + "下载成功");
                    }
                    else {
                        NotiCenterWindowViewModel.Current.Manager.ShowErrorMessage(message, 4);
                    }
                    downloadComplete?.Invoke(isSuccess, message, saveFileFullName);
                };
                OfficialServer.FileUrlService.GetNTMinerUrlAsync(fileName, (url, e) => {
                    webClient.DownloadFileAsync(new Uri(url), saveFileFullName);
                });
            }
        }

        public void Refresh() {
            OfficialServer.FileUrlService.GetNTMinerFilesAsync(App.AppType, (ntMinerFiles, e) => {
                this.NTMinerFiles = (ntMinerFiles ?? new List<NTMinerFileData>()).Select(a => new NTMinerFileViewModel(a)).OrderByDescending(a => a.VersionData).ToList();
                if (this.NTMinerFiles == null || this.NTMinerFiles.Count == 0) {
                    LocalIsLatest = true;
                }
                else {
                    ServerLatestVm = this.NTMinerFiles.OrderByDescending(a => a.VersionData).FirstOrDefault();
                    if (ServerLatestVm.VersionData > LocalNTMinerVersion) {
                        Logger.WarnDebugLine("发现新版本" + ServerLatestVm.Version);
                        this.SelectedNTMinerFile = ServerLatestVm;
                        LocalIsLatest = false;
                    }
                    else {
                        LocalIsLatest = true;
                    }
                }
                OnPropertyChanged(nameof(IsBtnInstallVisible));
                IsReady = true;
                if (!string.IsNullOrEmpty(CommandLineArgs.NTMinerFileName)) {
                    NTMinerFileViewModel ntminerFileVm = this.NTMinerFiles.FirstOrDefault(a => a.FileName == CommandLineArgs.NTMinerFileName);
                    if (ntminerFileVm != null) {
                        IsHistoryVisible = Visibility.Visible;
                        this.SelectedNTMinerFile = ntminerFileVm;
                        Install.Execute(null);
                    }
                }
            });
        }
        public BitmapImage BigLogoImageSource {
            get {
                return IconConst.BigLogoImageSource;
            }
        }

        public Visibility IsDebugModeVisible {
            get {
                if (DevMode.IsDebugMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public bool LocalIsLatest {
            get => _localIsLatest;
            set {
                _localIsLatest = value;
                OnPropertyChanged(nameof(LocalIsLatest));
            }
        }

        public Visibility IsHistoryVisible {
            get { return _isHistoryVisible; }
            set {
                _isHistoryVisible = value;
                OnPropertyChanged(nameof(IsHistoryVisible));
                OnPropertyChanged(nameof(BtnShowHistoryText));
                OnPropertyChanged(nameof(IsBtnInstallVisible));
            }
        }

        public string BtnShowHistoryText {
            get {
                if (this.IsHistoryVisible == Visibility.Visible) {
                    return "最新版本";
                }
                return "历史版本";
            }
        }

        public Visibility IsBtnInstallVisible {
            get {
                if (IsHistoryVisible == Visibility.Collapsed && LocalIsLatest) {
                    return Visibility.Collapsed;
                }
                if (SelectedNTMinerFile != null) {
                    return Visibility.Visible;
                }
                else {
                    return Visibility.Collapsed;
                }
            }
        }

        private Version _localNTMinerVersion;
        public Version LocalNTMinerVersion {
            get {
                if (_localNTMinerVersion == null) {
                    string currentVersion = NTMinerRegistry.GetCurrentVersion();
                    if (!Version.TryParse(currentVersion, out _localNTMinerVersion)) {
                        _localNTMinerVersion = new Version(1, 0);
                    }
                }
                return _localNTMinerVersion;
            }
        }

        public string LocalNTMinerVersionTag {
            get {
                return NTMinerRegistry.GetCurrentVersionTag();
            }
        }

        public Visibility BtnCancelVisible {
            get => _btnCancelVisible;
            set {
                _btnCancelVisible = value;
                OnPropertyChanged(nameof(BtnCancelVisible));
            }
        }

        public bool IsDownloading {
            get { return _isDownloading; }
            set {
                _isDownloading = value;
                OnPropertyChanged(nameof(IsDownloading));
            }
        }

        public double DownloadPercent {
            get {
                return _downloadPercent;
            }
            set {
                _downloadPercent = value;
                OnPropertyChanged(nameof(DownloadPercent));
            }
        }

        public string DownloadMessage {
            get {
                return _downloadMessage;
            }
            set {
                _downloadMessage = value;
                OnPropertyChanged(nameof(DownloadMessage));
            }
        }

        public List<NTMinerFileViewModel> NTMinerFiles {
            get => _nTMinerFiles;
            set {
                _nTMinerFiles = value;
                OnPropertyChanged(nameof(NTMinerFiles));
            }
        }

        public NTMinerFileViewModel SelectedNTMinerFile {
            get => _selectedNTMinerFile;
            set {
                _selectedNTMinerFile = value;
                OnPropertyChanged(nameof(SelectedNTMinerFile));
                OnPropertyChanged(nameof(IsBtnInstallVisible));
            }
        }

        public NTMinerFileViewModel ServerLatestVm {
            get {
                return _serverLatestVm;
            }
            set {
                _serverLatestVm = value;
                OnPropertyChanged(nameof(ServerLatestVm));
            }
        }

        public bool IsReady {
            get => _isReady;
            set {
                _isReady = value;
                OnPropertyChanged(nameof(IsReady));
            }
        }
    }
}
