namespace NTMiner.Vms {
    public class InnerPropertyViewModel : ViewModelBase {
        private string _serverJsonVersion;

        public InnerPropertyViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            _serverJsonVersion = NTMinerRoot.Instance.GetServerJsonVersion();
        }

        public string ServerJsonVersion {
            get => _serverJsonVersion;
            set {
                if (_serverJsonVersion != value) {
                    _serverJsonVersion = value;
                    OnPropertyChanged(nameof(ServerJsonVersion));
                }
            }
        }
    }
}
