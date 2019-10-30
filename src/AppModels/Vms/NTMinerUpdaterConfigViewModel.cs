using NTMiner.MinerServer;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class NTMinerUpdaterConfigViewModel : ViewModelBase {
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public NTMinerUpdaterConfigViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.Save = new DelegateCommand(() => {
                try {
                    if (string.IsNullOrEmpty(this.FileName)) {
                        this.FileName = NTKeyword.NTMinerUpdaterFileName;
                    }
                    VirtualRoot.Execute(new ChangeServerAppSettingCommand(new AppSettingData {
                        Key = NTKeyword.NTMinerUpdaterFileNameAppSettingKey,
                        Value = this.FileName
                    }));
                    CloseWindow?.Invoke();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
            });
            if (NTMinerRoot.Instance.ServerAppSettingSet.TryGetAppSetting(NTKeyword.NTMinerUpdaterFileNameAppSettingKey, out IAppSetting appSetting) && appSetting.Value != null) {
                _fileName = appSetting.Value.ToString();
            }
            else {
                _fileName = NTKeyword.NTMinerUpdaterFileName;
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
