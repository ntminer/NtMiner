using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.IO;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class NTMinerUpdaterConfigViewModel : ViewModelBase {
        public readonly Guid Id = Guid.NewGuid();
        public ICommand Save { get; private set; }

        public NTMinerUpdaterConfigViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Save = new DelegateCommand(() => {
                try {
                    if (string.IsNullOrEmpty(this.FileName)) {
                        this.FileName = HomePath.NTMinerUpdaterFileName;
                    }
                    VirtualRoot.Execute(new SetServerAppSettingCommand(new AppSettingData {
                        Key = NTKeyword.NTMinerUpdaterFileNameAppSettingKey,
                        Value = this.FileName
                    }));
                    VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
            });
            RpcRoot.OfficialServer.FileUrlService.GetNTMinerUpdaterUrlAsync((fileDownloadUrl, e) => {
                if (!string.IsNullOrEmpty(fileDownloadUrl)) {
                    Uri uri = new Uri(fileDownloadUrl);
                    FileName = Path.GetFileName(uri.LocalPath);
                }
                else {
                    FileName = HomePath.NTMinerUpdaterFileName;
                }
            });
        }

        private string _fileName;
        public string FileName {
            get {
                return _fileName;
            }
            set {
                if (_fileName != value) {
                    _fileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }
    }
}
