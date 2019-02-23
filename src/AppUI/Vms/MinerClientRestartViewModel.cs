using NTMiner.Notifications;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerClientRestartViewModel : ViewModelBase {
        private MinerClientViewModel _minerClientVm;
        private MineWorkViewModel _selectedMineWork;
        public ICommand Ok { get; private set; }

        public Action CloseWindow { get; set; }

        public MinerClientRestartViewModel(MinerClientViewModel minerClientVm) {
            this.Ok = new DelegateCommand(() => {
                Server.MinerClientService.RestartNTMinerAsync(minerClientVm.MinerIp, SelectedMineWork.Id, response => {
                    if (!response.IsSuccess()) {
                        if (response != null) {
                            Write.UserLine(response.Description, ConsoleColor.Red);
                            MinerClientsWindowViewModel.Current.Manager.CreateMessage()
                                 .Accent("#1751C3")
                                 .Background("Red")
                                 .HasBadge("Error")
                                 .HasMessage(response.Description)
                                 .Dismiss()
                                 .WithDelay(TimeSpan.FromSeconds(5))
                                 .Queue();
                        }
                    }
                });
                CloseWindow?.Invoke();
            });
            _minerClientVm = minerClientVm;
            _selectedMineWork = MineWorkVms.MineWorkItems.FirstOrDefault(a => a.Id == minerClientVm.WorkId);
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
