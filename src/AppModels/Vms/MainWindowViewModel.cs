using NTMiner.Core;
using NTMiner.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        public class LogFile {
            public LogFile(string fileName, DateTime lastWriteTime, string fileFullName) {
                this.FileName = fileName;
                this.LastWriteTime = lastWriteTime;
                this.LastWriteTimeText = Timestamp.GetTimestampText(lastWriteTime);
                this.FileFullName = fileFullName;
            }

            public string FileFullName { get; private set; }
            public string FileName { get; private set; }
            public DateTime LastWriteTime { get; private set; }
            public string LastWriteTimeText { get; private set; }
        }

        private readonly StateBarViewModel _stateBarVm = new StateBarViewModel();
        private List<LogFile> _logFiles;
        private LogFile _selectedLogFile;

        public ICommand UseThisPcName { get; private set; }
        public ICommand CloseMainWindow { get; private set; }
        public ICommand OpenLogFile { get; private set; }

        public MainWindowViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.CloseMainWindow = new DelegateCommand(() => {
                VirtualRoot.Execute(new CloseMainWindowCommand(isAutoNoUi: false));
            });
            this.UseThisPcName = new DelegateCommand(() => {
                string thisPcName = NTMinerRoot.ThisPcName;
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定使用本机名{thisPcName}作为矿机名吗？", title: "确认", onYes: () => {
                    MinerProfile.MinerName = thisPcName;
                }));
            });
            this.OpenLogFile = new DelegateCommand<string>((fileFullName) => {
                OpenLogFileByNpp(fileFullName);
            });
        }

        public bool IsTestHost {
            get {
                return !string.IsNullOrEmpty(Hosts.GetIp(NTKeyword.ServerHost, out long _));
            }
            set {
                if (value) {
                    Hosts.SetHost(NTKeyword.ServerHost, "127.0.0.1");
                }
                else {
                    Hosts.SetHost(NTKeyword.ServerHost, string.Empty);
                }
                OnPropertyChanged(nameof(IsTestHost));
            }
        }

        public string BrandTitle {
            get {
                if (NTMinerRoot.KernelBrandId == Guid.Empty && NTMinerRoot.PoolBrandId == Guid.Empty) {
                    return string.Empty;
                }
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTMinerRoot.KernelBrandId, out ISysDicItem dicItem)) {
                    if (!string.IsNullOrEmpty(dicItem.Value)) {
                        return dicItem.Value + "专版";
                    }
                    return dicItem.Code + "专版";
                }
                else if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTMinerRoot.PoolBrandId, out dicItem)) {
                    if (!string.IsNullOrEmpty(dicItem.Value)) {
                        return dicItem.Value + "专版";
                    }
                    return dicItem.Code + "专版";
                }
                return string.Empty;
            }
        }

        public StateBarViewModel StateBarVm {
            get => _stateBarVm;
        }

        public bool IsUseDevConsole {
            get { return NTMinerRoot.IsUseDevConsole; }
            set {
                NTMinerRoot.IsUseDevConsole = value;
                OnPropertyChanged(nameof(IsUseDevConsole));
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Instance.MinerProfileVm;
            }
        }

        public List<LogFile> LogFiles {
            get => _logFiles;
            set {
                _logFiles = value;
                OnPropertyChanged(nameof(LogFiles));
            }
        }

        public LogFile SelectedLogFile {
            get => _selectedLogFile;
            set {
                _selectedLogFile = value;
                OnPropertyChanged(nameof(SelectedLogFile));
            }
        }

        public void RefreshLogFiles() {
            if (!Directory.Exists(Logger.Dir)) {
                return;
            }
            this.LogFiles = Directory.GetFiles(Logger.Dir).Select(a => {
                FileInfo fileInfo = new FileInfo(a);
                return new LogFile(fileInfo.Name, fileInfo.LastWriteTime, fileFullName: a);
            }).OrderByDescending(a => a.LastWriteTime).ToList();
            this.SelectedLogFile = this.LogFiles.FirstOrDefault(a => !a.FileName.StartsWith("root"));
        }

        public string GetLatestLogFileFullName() {
            if (!Directory.Exists(Logger.Dir)) {
                return null;
            }
            string fileFullName = null;
            try {
                string latestOne = null;
                DateTime lastWriteTime = DateTime.MinValue;
                FileInfo fileInfo;
                foreach (var itemFullName in Directory.GetFiles(Logger.Dir)) {
                    fileInfo = new FileInfo(itemFullName);
                    if (fileInfo.Name.StartsWith("root")) {
                        continue;
                    }
                    if (fileInfo.LastWriteTime > lastWriteTime) {
                        lastWriteTime = fileInfo.LastWriteTime;
                        latestOne = itemFullName;
                    }
                }
                fileFullName = latestOne;
            }
            catch {
            }
            return fileFullName;
        }

        public void OpenLogFileByNpp(string fileFullName) {
            string nppDir = Path.Combine(SpecialPath.ToolsDirFullName, "Npp");
            string nppFileFullName = Path.Combine(nppDir, "notepad++.exe");
            if (!Directory.Exists(nppDir)) {
                Directory.CreateDirectory(nppDir);
            }
            if (!File.Exists(nppFileFullName)) {
                VirtualRoot.Execute(new ShowFileDownloaderCommand(AppStatic.NppPackageUrl, "Notepad++", (window, isSuccess, message, saveFileFullName) => {
                    if (isSuccess) {
                        ZipUtil.DecompressZipFile(saveFileFullName, nppDir);
                        File.Delete(saveFileFullName);
                        window?.Close();
                        Windows.Cmd.RunClose(nppFileFullName, $"-nosession -ro {fileFullName}");
                    }
                }));
            }
            else {
                Windows.Cmd.RunClose(nppFileFullName, $"-nosession -ro {fileFullName}");
            }
        }
    }
}
