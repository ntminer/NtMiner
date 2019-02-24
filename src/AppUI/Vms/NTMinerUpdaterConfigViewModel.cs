using NTMiner.MinerServer;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class NTMinerUpdaterConfigViewModel : ViewModelBase {
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public NTMinerUpdaterConfigViewModel() {
            this.Save = new DelegateCommand(() => {
                try {
                    if (string.IsNullOrEmpty(this.FileName)) {
                        this.FileName = "NTMinerUpdater.exe";
                    }
                    VirtualRoot.Execute(new ChangeAppSettingCommand(new AppSettingData {
                        Key = "ntminerUpdaterFileName",
                        Value = this.FileName
                    }));
                    CloseWindow?.Invoke();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
            });
            _fileName = "NTMinerUpdater.exe";
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
