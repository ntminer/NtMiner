using Microsoft.Win32;
using NTMiner.ServiceContracts.DataObjects;
using NTMiner.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

        private static readonly string _ntminerDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
        private static readonly string _downloadDirFullName = Path.Combine(_ntminerDirFullName, "Temp", "Download");
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
                object locationValue = Windows.Registry.GetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "Location");
                object argumentsValue = Windows.Registry.GetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "Arguments");
                if (locationValue == null) {
                    return;
                }
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
                        Task.Factory.StartNew(CloseNTMinerMainWindow);
                        TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                            string location = (string)locationValue;
                            File.Copy(saveFileFullName, location, overwrite: true);
                            File.Delete(saveFileFullName);
                            Process.Start(location, (string)argumentsValue);
                            this.IsDownloading = false;
                            Execute.OnUIThread(() => {
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
                NTMinerFileEdit window = new NTMinerFileEdit("添加", "Icon_Add", new NTMinerFileViewModel());
                window.ShowDialogEx();
            });
        }

        public void Refresh() {
            Server.FileUrlService.GetNTMinerFiles(ntMinerFiles => {
                this.NTMinerFiles = (ntMinerFiles ?? new List<NTMinerFileData>()).Select(a => new NTMinerFileViewModel(a)).ToList();
                if (this.NTMinerFiles == null || this.NTMinerFiles.Count == 0) {
                    LocalIsLatest = true;
                }
                else {
                    ServerLatestVm = this.NTMinerFiles.OrderByDescending(a => a.VersionData).FirstOrDefault();
                    if (ServerLatestVm.VersionData > LocalNTMinerVersion) {
                        Global.Logger.Info("发现新版本" + ServerLatestVm.Version);
                        this.SelectedNTMinerFile = ServerLatestVm;
                        LocalIsLatest = false;
                    }
                    else {
                        LocalIsLatest = true;
                    }
                }
                OnPropertyChanged(nameof(IsBtnInstallVisible));
                IsReady = true;
            });
        }

        private static NamedPipeClientStream _pipeClient;
        private static void CloseNTMinerMainWindow() {
            try {
                _pipeClient = new NamedPipeClientStream(".", "ntminerclient", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);
                _pipeClient.Connect(200);
                StreamWriter sw = new StreamWriter(_pipeClient);
                sw.WriteLine("CloseMainWindow");
                sw.Flush();
                Thread.Sleep(1000);
                sw.Close();
            }
            catch (Exception ex) {
                Global.Logger.Error(ex.Message, ex);
            }
        }

        public Visibility IsDevModeVisible {
            get {
                if (DevMode.IsDevMode) {
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
                    string currentVersion = GetLocalNTMinerVersion();
                    if (string.IsNullOrEmpty(currentVersion)) {
                        currentVersion = "1.0";
                    }
                    if (!Version.TryParse(currentVersion, out _localNTMinerVersion)) {
                        _localNTMinerVersion = new Version(1, 0);
                    }
                }
                return _localNTMinerVersion;
            }
        }

        public string LocalNTMinerVersionTag {
            get {
                return GetLocalNTMinerVersionTag();
            }
        }

        private static string GetLocalNTMinerVersion() {
            string currentVersion = "1.0.0.0";
            string currentVersionTag = string.Empty;
            object currentVersionValue = Windows.Registry.GetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "CurrentVersion");
            if (currentVersionValue != null) {
                currentVersion = (string)currentVersionValue;
            }
            return currentVersion;
        }

        private static string GetLocalNTMinerVersionTag() {
            string currentVersionTag = string.Empty;
            object currentVersionTagValue = Windows.Registry.GetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "CurrentVersionTag");
            if (currentVersionTagValue != null) {
                currentVersionTag = (string)currentVersionTagValue;
            }
            return currentVersionTag;
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

        public static void Download(
            string fileName,
            string version,
            Action<int> progressChanged,
            Action<bool, string, string> downloadComplete,
            out Action cancel) {
            Global.DebugLine("下载：" + fileName);
            string saveFileFullName = Path.Combine(_downloadDirFullName, "NTMiner" + version);
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
                    if (isSuccess) {
                        Global.Logger.Info("NTMiner" + version + "下载成功");
                    }
                    string message = "下载成功";
                    if (e.Error != null) {
                        message = "下载失败";
                        Global.Logger.Error(e.Error.Message, e.Error);
                    }
                    if (e.Cancelled) {
                        message = "下载取消";
                    }
                    downloadComplete?.Invoke(isSuccess, message, saveFileFullName);
                };
                Server.FileUrlService.GetNTMinerUrl(fileName, url => {
                    webClient.DownloadFileAsync(new Uri(url), saveFileFullName);
                });
            }
        }
    }
}
