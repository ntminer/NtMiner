using NTMiner.Notifications;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerClientRestartViewModel : ViewModelBase {
        private string _title;
        private MineWorkViewModel _selectedMineWork;
        public ICommand Ok { get; private set; }

        public Action CloseWindow { get; set; }

        public MinerClientRestartViewModel(string title, Guid workId, Action onOk) {
            this.Ok = new DelegateCommand(() => {
                onOk?.Invoke();
            });
            _title = title;
            _selectedMineWork = MineWorkVms.MineWorkItems.FirstOrDefault(a => a.Id == workId);
        }

        public string Title {
            get { return _title; }
            set {
                _title = value;
                OnPropertyChanged(nameof(Title));
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
