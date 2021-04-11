using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class LogFilesViewModel : ViewModelBase {
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
        private List<LogFile> _logFiles;
        private LogFile _selectedLogFile;
        public ICommand OpenLogFile { get; private set; }

        public LogFilesViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.OpenLogFile = new DelegateCommand<string>((fileFullName) => {
                OpenLogFileByEveredit(fileFullName);
            });
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
            if (!Directory.Exists(Logger.DirFullPath)) {
                return;
            }
            this.LogFiles = Directory.GetFiles(Logger.DirFullPath).Select(a => {
                FileInfo fileInfo = new FileInfo(a);
                return new LogFile(fileInfo.Name, fileInfo.LastWriteTime, fileFullName: a);
            }).OrderByDescending(a => a.LastWriteTime).ToList();
            this.SelectedLogFile = this.LogFiles.FirstOrDefault(a => !a.FileName.StartsWith("root"));
        }

        public string GetLatestLogFileFullName() {
            if (!Directory.Exists(Logger.DirFullPath)) {
                return null;
            }
            string fileFullName = null;
            try {
                string latestOne = null;
                DateTime lastWriteTime = DateTime.MinValue;
                FileInfo fileInfo;
                foreach (var itemFullName in Directory.GetFiles(Logger.DirFullPath)) {
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
            OpenLogFileBy(
                downloadFileUrl: AppRoot.NppPackageUrl,
                toolSubDirName: "Npp",
                exeFileName: "notepad++.exe",
                downloadTitle: "Notepad++",
                args: $"-nosession -ro {fileFullName}");
        }

        // 用户查看挖矿日志的时候会下载使用一个文本编辑器，因为日志文件可能很大直接使用windows自带
        // 的记事本打开会很耗资源，不要使用notepad++，EverEdit是一个很好的替代，而且是国人开发的。
        public void OpenLogFileByEveredit(string fileFullName) {
            OpenLogFileBy(
                downloadFileUrl: AppRoot.EvereditPackageUrl,
                toolSubDirName: "everedit",
                exeFileName: "EverEdit.exe",
                downloadTitle: "EverEdit",
                args: fileFullName);
        }

        private static void OpenLogFileBy(string downloadFileUrl, string toolSubDirName, string exeFileName, string downloadTitle, string args) {
            string dir = Path.Combine(MinerClientTempPath.ToolsDirFullName, toolSubDirName);
            string exeFileFullName = Path.Combine(dir, exeFileName);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(exeFileFullName)) {
                VirtualRoot.Execute(new ShowFileDownloaderCommand(downloadFileUrl, downloadTitle, (window, isSuccess, message, saveFileFullName) => {
                    if (isSuccess) {
                        ZipUtil.DecompressZipFile(saveFileFullName, dir);
                        File.Delete(saveFileFullName);
                        window?.Close();
                        Windows.Cmd.RunClose(exeFileFullName, args);
                    }
                }));
            }
            else {
                Windows.Cmd.RunClose(exeFileFullName, args);
            }
        }
    }
}
