using NTMiner.Notifications;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {

    public class MainWindowViewModel : ViewModelBase {
        public static readonly MainWindowViewModel Current = new MainWindowViewModel();

        private INotificationMessageManager _manager;
        private Visibility _isBtnRunAsAdministratorVisible = Visibility.Collapsed;
        private bool _isDaemonRunning = true;

        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }

        private MainWindowViewModel() {
            this.StartMine = new DelegateCommand(() => {
                NTMinerRoot.Current.StartMine(CommandLineArgs.WorkId);
            });
            this.StopMine = new DelegateCommand(() => {
                NTMinerRoot.Current.StopMine();
            });
            if (DevMode.IsDevMode) {
                Global.Access<Per10SecondEvent>(
                    Guid.Parse("868658E4-B281-4E55-BE0F-0E2B66777D6C"),
                    "在界面上展示守护进程的运行状态",
                    LogEnum.None,
                    action: message => {
                        NTMinerClientDaemon.Instance.GetDaemonVersion(Global.Localhost, Global.ClientPort, thatVersion => {
                            this.IsDaemonRunning = !string.IsNullOrEmpty(thatVersion);
                        });
                    });
            }
        }

        public double Height {
            get {
                if (SystemParameters.WorkArea.Size.Height > 620) {
                    if (DevMode.IsDevMode) {
                        return 920;
                    }
                    return 620;
                }
                if (DevMode.IsDevMode) {
                    return 820;
                }
                return 520;
            }
        }

        public double Width {
            get {
                if (SystemParameters.WorkArea.Size.Width > 1000) {
                    return 1000;
                }
                return 860;
            }
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

        public bool IsDaemonRunning {
            get { return _isDaemonRunning; }
            set {
                _isDaemonRunning = value;
                OnPropertyChanged(nameof(IsDaemonRunning));
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
