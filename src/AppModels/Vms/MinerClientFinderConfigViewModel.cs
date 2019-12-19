using NTMiner.Core;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
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
            if (NTMinerRoot.Instance.ServerAppSettingSet.TryGetAppSetting(NTKeyword.MinerClientFinderFileNameAppSettingKey, out IAppSetting appSetting) && appSetting.Value != null) {
                _fileName = appSetting.Value.ToString();
            }
            else {
                _fileName = NTKeyword.MinerClientFinderFileName;
            }
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
