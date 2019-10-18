namespace NTMiner.Vms {
    using NTMiner.MinerClient;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class WorkerEventsViewModel : ViewModelBase {
        private readonly ObservableCollection<WorkerEventViewModel> _workerEventVms;
        private EnumItem<WorkerEventChannel> _selectedChannel;
        private int _errorCount;
        private int _warnCount;
        private int _infoCount;

        public WorkerEventsViewModel() {
            var data = VirtualRoot.WorkerEvents.GetEvents().OrderByDescending(a => a.Id).Select(a => new WorkerEventViewModel(a)).ToArray();
            foreach (var item in data) {
                switch (item.EventType) {
                    case nameof(WorkerEventType.Error):
                        _errorCount++;
                        break;
                    case nameof(WorkerEventType.Warn):
                        _warnCount++;
                        break;
                    case nameof(WorkerEventType.Info):
                        _infoCount++;
                        break;
                    default:
                        break;
                }
            }
            _workerEventVms = new ObservableCollection<WorkerEventViewModel>(data);
            _selectedChannel = WorkerEventChannel.Unspecified.GetEnumItem();
        }

        public EnumItem<WorkerEventChannel> SelectedChannel {
            get => _selectedChannel;
            set {
                if (_selectedChannel != value) {
                    _selectedChannel = value;
                    OnPropertyChanged(nameof(SelectedChannel));
                }
            }
        }

        public int ErrorCount {
            get => _errorCount;
            set {
                _errorCount = value;
                OnPropertyChanged(nameof(ErrorCount));
            }
        }
        public int WarnCount {
            get => _warnCount;
            set {
                _warnCount = value;
                OnPropertyChanged(nameof(WarnCount));
            }
        }
        public int InfoCount {
            get => _infoCount;
            set {
                _infoCount = value;
                OnPropertyChanged(nameof(InfoCount));
            }
        }

        public void RefreshCount(IWorkerEvent data) {
            switch (data.EventType) {
                case nameof(WorkerEventType.Error):
                    ErrorCount++;
                    break;
                case nameof(WorkerEventType.Warn):
                    WarnCount++;
                    break;
                case nameof(WorkerEventType.Info):
                    InfoCount++;
                    break;
                default:
                    break;
            }
        }

        public ObservableCollection<WorkerEventViewModel> WorkerEventVms {
            get {
                return _workerEventVms;
            }
        }
    }
}
