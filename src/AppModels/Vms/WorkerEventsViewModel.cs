namespace NTMiner.Vms {
    using System.Collections.ObjectModel;
    using System.Linq;

    public class WorkerEventsViewModel : ViewModelBase {
        private readonly ObservableCollection<WorkerEventViewModel> _workerEventVms;
        private ObservableCollection<WorkerEventViewModel> _queyResults;
        private EnumItem<WorkerEventChannel> _selectedChannel;
        private int _errorCount;
        private int _warnCount;
        private int _infoCount;
        private bool _isErrorChecked = true;
        private bool _isWarnChecked = true;
        private bool _isInfoChecked = true;

        public WorkerEventsViewModel() {
            var data = VirtualRoot.WorkerEvents.Select(a => new WorkerEventViewModel(a));
            _workerEventVms = new ObservableCollection<WorkerEventViewModel>(data);
            _queyResults = _workerEventVms;
            foreach (var item in _workerEventVms) {
                switch (item.EventTypeEnum) {
                    case WorkerEventType.Error:
                        _errorCount++;
                        break;
                    case WorkerEventType.Warn:
                        _warnCount++;
                        break;
                    case WorkerEventType.Info:
                        _infoCount++;
                        break;
                    default:
                        break;
                }
            }
            _selectedChannel = WorkerEventChannel.Unspecified.GetEnumItem();

            VirtualRoot.CreateEventPath<WorkerEvent>("发生了挖矿事件后刷新Vm内存", LogEnum.DevConsole,
                action: message => {
                    var vm = new WorkerEventViewModel(message.Source);
                    _workerEventVms.Insert(0, vm);
                    switch (vm.EventTypeEnum) {
                        case WorkerEventType.Error:
                            ErrorCount++;
                            break;
                        case WorkerEventType.Warn:
                            WarnCount++;
                            break;
                        case WorkerEventType.Info:
                            InfoCount++;
                            break;
                        default:
                            break;
                    }

                    #region 更新QueryResults
                    bool isCurrentChannel = SelectedChannel.Value == WorkerEventChannel.Unspecified;
                    bool isEventTypeChecked = IsErrorChecked && IsWarnChecked && IsInfoChecked;
                    if (!isCurrentChannel) {
                        switch (vm.ChannelEnum) {
                            case WorkerEventChannel.Kernel:
                                isCurrentChannel = SelectedChannel.Value == WorkerEventChannel.Kernel;
                                break;
                            case WorkerEventChannel.Server:
                                isCurrentChannel = SelectedChannel.Value == WorkerEventChannel.Server;
                                break;
                            case WorkerEventChannel.This:
                                isCurrentChannel = SelectedChannel.Value == WorkerEventChannel.This;
                                break;
                            case WorkerEventChannel.Unspecified:
                                isCurrentChannel = true;
                                break;
                            default:
                                break;
                        }
                    }
                    if (!isEventTypeChecked) {
                        switch (vm.EventTypeEnum) {
                            case WorkerEventType.Error:
                                isEventTypeChecked = IsErrorChecked;
                                break;
                            case WorkerEventType.Warn:
                                isEventTypeChecked = IsWarnChecked;
                                break;
                            case WorkerEventType.Info:
                                isEventTypeChecked = IsInfoChecked;
                                break;
                            default:
                                break;
                        }
                    }
                    if (isCurrentChannel && isEventTypeChecked) {
                        _queyResults.Insert(0, vm);
                    }
                    #endregion
                });
        }

        public EnumItem<WorkerEventChannel> SelectedChannel {
            get => _selectedChannel;
            set {
                if (_selectedChannel != value) {
                    _selectedChannel = value;
                    OnPropertyChanged(nameof(SelectedChannel));
                    RefreshQueryResults();
                }
            }
        }

        public bool IsErrorChecked {
            get => _isErrorChecked;
            set {
                _isErrorChecked = value;
                OnPropertyChanged(nameof(IsErrorChecked));
                RefreshQueryResults();
            }
        }
        public bool IsWarnChecked {
            get => _isWarnChecked;
            set {
                _isWarnChecked = value;
                OnPropertyChanged(nameof(_isWarnChecked));
                RefreshQueryResults();
            }
        }
        public bool IsInfoChecked {
            get => _isInfoChecked;
            set {
                _isInfoChecked = value;
                OnPropertyChanged(nameof(IsInfoChecked));
                RefreshQueryResults();
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

        public ObservableCollection<WorkerEventViewModel> QueryResults {
            get {
                return _queyResults;
            }
        }

        private void RefreshQueryResults() {
            var query = _workerEventVms.AsQueryable();
            if (SelectedChannel.Value != WorkerEventChannel.Unspecified) {
                string channel = SelectedChannel.Value.GetName();
                query = query.Where(a => a.Channel == channel);
            }
            if (!IsErrorChecked || !IsWarnChecked || !IsInfoChecked) {
                query = query.Where(a => a.EventTypeEnum == WorkerEventType.Undefined
                    || (a.EventTypeEnum == WorkerEventType.Error && IsErrorChecked)
                    || (a.EventTypeEnum == WorkerEventType.Warn && IsWarnChecked)
                    || (a.EventTypeEnum == WorkerEventType.Info && IsInfoChecked));
            }
            _queyResults = new ObservableCollection<WorkerEventViewModel>(query);
            OnPropertyChanged(nameof(QueryResults));
        }
    }
}
