using NTMiner.Core.MinerServer;
using NTMiner.Vms;

namespace NTMiner.MinerStudio.Vms {
    public class CoinSnapshotViewModel : ViewModelBase {
        public static readonly CoinSnapshotViewModel PleaseSelect = new CoinSnapshotViewModel(CoinViewModel.PleaseSelect, new CoinSnapshotDataViewModel(CoinSnapshotData.CreateEmpty(CoinViewModel.PleaseSelect.Code)));

        private CoinViewModel _coinVm;
        private CoinSnapshotDataViewModel _coinSnapshotDataVm;

        public CoinSnapshotViewModel(CoinViewModel coinVm, CoinSnapshotDataViewModel coinSnapshotDataVm) {
            _coinVm = coinVm;
            _coinSnapshotDataVm = coinSnapshotDataVm;
        }

        public CoinViewModel CoinVm {
            get => _coinVm;
            set {
                _coinVm = value;
                OnPropertyChanged(nameof(CoinVm));
            }
        }

        public CoinSnapshotDataViewModel CoinSnapshotDataVm {
            get => _coinSnapshotDataVm;
            set {
                _coinSnapshotDataVm = value;
                OnPropertyChanged(nameof(CoinSnapshotDataVm));
            }
        }

        public bool IsPleaseSelect {
            get {
                return this == PleaseSelect;
            }
        }
    }
}
