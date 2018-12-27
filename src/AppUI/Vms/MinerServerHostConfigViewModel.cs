namespace NTMiner.Vms {
    public class MinerServerHostConfigViewModel : ViewModelBase {
        public MinerServerHostConfigViewModel() {
            _minerServerHost = Server.MinerServerHost;
        }

        private string _minerServerHost;
        public string MinerServerHost {
            get {
                return _minerServerHost;
            }
            set {
                if (_minerServerHost != value) {
                    _minerServerHost = value;
                    OnPropertyChanged(nameof(MinerServerHost));
                }
            }
        }

        public int MinerServerPort {
            get {
                return Server.MinerServerPort;
            }
        }
    }
}
