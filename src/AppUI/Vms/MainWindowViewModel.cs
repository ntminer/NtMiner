using NTMiner.Notifications;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {

    public class MainWindowViewModel : ViewModelBase {
        public static readonly MainWindowViewModel Current = new MainWindowViewModel();

        private INotificationMessageManager _manager;
        private Visibility _isBtnRunAsAdministratorVisible = Visibility.Collapsed;

        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }

        private MainWindowViewModel() {
            this.StartMine = new DelegateCommand(() => {
                NTMinerRoot.Current.StartMine(CommandLineArgs.WorkId);
            });
            this.StopMine = new DelegateCommand(() => {
                NTMinerRoot.Current.StopMine();
            });
        }

        public bool JustClientWorker {
            get {
                return CommandLineArgs.JustClientWorker;
            }
        }

        public Visibility IsBtnRunAsAdministratorVisible {
            get => _isBtnRunAsAdministratorVisible;
            set {
                _isBtnRunAsAdministratorVisible = value;
                OnPropertyChanged(nameof(IsBtnRunAsAdministratorVisible));
            }
        }

        public LangViewModels LangVms {
            get {
                return LangViewModels.Current;
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }

        public Guid WorkId {
            get {
                return CommandLineArgs.WorkId;
            }
        }

        public INotificationMessageManager Manager {
            get {
                if (_manager == null) {
                    _manager = new NotificationMessageManager();
                }
                return _manager;
            }
        }
    }
}
