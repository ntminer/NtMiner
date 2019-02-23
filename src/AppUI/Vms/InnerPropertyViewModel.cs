using System;
using System.IO;

namespace NTMiner.Vms {
    public class InnerPropertyViewModel : ViewModelBase {
        public static readonly InnerPropertyViewModel Current = new InnerPropertyViewModel();

        private string _appName;
        private DateTime _bootOn;
        private string _globalDir;
        private string _langDbFileFullName;
        private string _serverDbFileFullName;
        private string _localDbFileFullName;

        public InnerPropertyViewModel() {
            _appName = NTMinerRoot.AppName;
            _bootOn = NTMinerRoot.Current.CreatedOn;
            _globalDir = ClientId.GlobalDirFullName;
            _langDbFileFullName = Path.Combine(ClientId.GlobalDirFullName, "lang.litedb");
            _serverDbFileFullName = Path.Combine(ClientId.GlobalDirFullName, "server.litedb");
            _localDbFileFullName = Path.Combine(ClientId.GlobalDirFullName, "local.litedb");
        }

        public string AppName {
            get => _appName;
            set {
                _appName = value;
                OnPropertyChanged(nameof(AppName));
            }
        }
        public DateTime BootOn {
            get => _bootOn;
            set {
                _bootOn = value;
                OnPropertyChanged(nameof(BootOn));
            }
        }
        public LangViewModels LangVms {
            get => LangViewModels.Current;
        }
        public string GlobalDir {
            get => _globalDir;
            set {
                _globalDir = value;
                OnPropertyChanged(nameof(GlobalDir));
            }
        }
        public string LangDbFileFullName {
            get => _langDbFileFullName;
            set {
                _langDbFileFullName = value;
                OnPropertyChanged(nameof(LangDbFileFullName));
            }
        }
        public string ServerDbFileFullName {
            get => _serverDbFileFullName;
            set {
                _serverDbFileFullName = value;
                OnPropertyChanged(nameof(ServerDbFileFullName));
            }
        }
        public string LocalDbFileFullName {
            get => _localDbFileFullName;
            set {
                _localDbFileFullName = value;
                OnPropertyChanged(nameof(LocalDbFileFullName));
            }
        }
    }
}
