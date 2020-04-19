using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.IO;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class MinerClientFinderConfigViewModel : ViewModelBase {
        public readonly Guid Id = Guid.NewGuid();
        public ICommand Save { get; private set; }

        public MinerClientFinderConfigViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Save = new DelegateCommand(() => {
                try {
                    if (string.IsNullOrEmpty(this.FileName)) {
                        this.FileName = NTKeyword.MinerClientFinderFileName;
                    }
                    VirtualRoot.Execute(new SetServerAppSettingCommand(new AppSettingData {
                        Key = NTKeyword.MinerClientFinderFileNameAppSettingKey,
                        Value = this.FileName
                    }));
                    VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
            });
            RpcRoot.OfficialServer.FileUrlService.GetMinerClientFinderUrlAsync((fileDownloadUrl, e) => {
                if (!string.IsNullOrEmpty(fileDownloadUrl)) {
                    Uri uri = new Uri(fileDownloadUrl);
                    _fileName = Path.GetFileName(uri.LocalPath);
                }
                else {
                    _fileName = NTKeyword.MinerClientFinderFileName;
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
