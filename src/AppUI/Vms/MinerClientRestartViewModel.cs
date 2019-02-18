using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerClientRestartViewModel : ViewModelBase {
        private MinerClientViewModel _minerClientVm;
        private MineWorkViewModel _selectedMineWork;
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public MinerClientRestartViewModel(MinerClientViewModel minerClientVm) {
            this.Save = new DelegateCommand(() => {
                // TODO:通过中控调用
                NTMinerDaemonService.Instance.RestartNTMinerAsync(minerClientVm.ClientDataVm.MinerIp, this.SelectedMineWork.Id, null);
                CloseWindow?.Invoke();
            });
            _minerClientVm = minerClientVm;
            _selectedMineWork = MineWorkVms.MineWorkItems.FirstOrDefault(a => a.Id == minerClientVm.ClientDataVm.WorkId);
        }

        public MinerClientViewModel MinerClientVm {
            get => _minerClientVm;
            set {
                if (_minerClientVm != value) {
                    _minerClientVm = value;
                    OnPropertyChanged(nameof(MinerClientVm));
                }
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
                if (_selectedMineWork != value) {
                    _selectedMineWork = value;
                    OnPropertyChanged(nameof(SelectedMineWork));
                }
            }
        }
    }
}
