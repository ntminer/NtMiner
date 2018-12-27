using System.Linq;

namespace NTMiner.Vms {
    public class MinerClientRestartViewModel : ViewModelBase {
        private MinerClientViewModel _minerClientVm;
        private MineWorkViewModel _selectedMineWork;

        public MinerClientRestartViewModel(MinerClientViewModel minerClientVm) {
            _minerClientVm = minerClientVm;
            _selectedMineWork = MineWorkVms.MineWorkItems.FirstOrDefault(a => a.Id == minerClientVm.ClientDataVm.WorkId);
        }

        public MinerClientViewModel MinerClientVm {
            get => _minerClientVm;
            set {
                _minerClientVm = value;
                OnPropertyChanged(nameof(MinerClientVm));
            }
        }

        public MineWorkViewModels MineWorkVms {
            get {
                return MineWorkViewModels.Current;
            }
        }

        public MineWorkViewModel SelectedMineWork {
            get => _selectedMineWork;
            set {
                _selectedMineWork = value;
                OnPropertyChanged(nameof(SelectedMineWork));
            }
        }
    }
}
