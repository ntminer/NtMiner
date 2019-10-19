namespace NTMiner.Vms {
    using NTMiner.MinerServer;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    public class WorkerMessagesViewModel : ViewModelBase {
        private readonly ObservableCollection<WorkerMessageViewModel> _workerMessageVms;
        private ObservableCollection<WorkerMessageViewModel> _queyResults;
        private EnumItem<WorkerMessageChannel> _selectedChannel;
        private int _errorCount;
        private int _warnCount;
        private int _infoCount;
        private string _keyword;

        public ICommand ClearKeyword { get; private set; }

        public WorkerMessagesViewModel() {
            var data = VirtualRoot.WorkerMessages.Select(a => new WorkerMessageViewModel(a));
            _workerMessageVms = new ObservableCollection<WorkerMessageViewModel>(data);
            _queyResults = _workerMessageVms;
            foreach (var item in _workerMessageVms) {
                switch (item.MessageTypeEnum) {
                    case WorkerMessageType.Error:
                        _errorCount++;
                        break;
                    case WorkerMessageType.Warn:
                        _warnCount++;
                        break;
                    case WorkerMessageType.Info:
                        _infoCount++;
                        break;
                    default:
                        break;
                }
            }
            _selectedChannel = WorkerMessageChannel.Unspecified.GetEnumItem();

            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
            VirtualRoot.CreateEventPath<WorkerMessage>("发生了挖矿事件后刷新Vm内存", LogEnum.DevConsole,
                action: message => {
                    var vm = new WorkerMessageViewModel(message.Source);
                    _workerMessageVms.Insert(0, vm);
                    switch (vm.MessageTypeEnum) {
                        case WorkerMessageType.Error:
                            ErrorCount++;
                            break;
                        case WorkerMessageType.Warn:
                            WarnCount++;
                            break;
                        case WorkerMessageType.Info:
                            InfoCount++;
                            break;
                        default:
                            break;
                    }

                    if (_queyResults != _workerMessageVms) {
                        #region 更新QueryResults
                        bool isCurrentChannel = SelectedChannel.Value == WorkerMessageChannel.Unspecified;
                        bool isMessageTypeChecked = IsErrorChecked && IsWarnChecked && IsInfoChecked;
                        if (!isCurrentChannel) {
                            switch (vm.ChannelEnum) {
                                case WorkerMessageChannel.Kernel:
                                    isCurrentChannel = SelectedChannel.Value == WorkerMessageChannel.Kernel;
                                    break;
                                case WorkerMessageChannel.Server:
                                    isCurrentChannel = SelectedChannel.Value == WorkerMessageChannel.Server;
                                    break;
                                case WorkerMessageChannel.This:
                                    isCurrentChannel = SelectedChannel.Value == WorkerMessageChannel.This;
                                    break;
                                case WorkerMessageChannel.Unspecified:
                                    isCurrentChannel = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (!isMessageTypeChecked) {
                            switch (vm.MessageTypeEnum) {
                                case WorkerMessageType.Error:
                                    isMessageTypeChecked = IsErrorChecked;
                                    break;
                                case WorkerMessageType.Warn:
                                    isMessageTypeChecked = IsWarnChecked;
                                    break;
                                case WorkerMessageType.Info:
                                    isMessageTypeChecked = IsInfoChecked;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (isCurrentChannel && isMessageTypeChecked) {
                            _queyResults.Insert(0, vm);
                        }
                        #endregion
                    }
                });
        }

        public string Keyword {
            get => _keyword;
            set {
                if (_keyword != value) {
                    _keyword = value;
                    OnPropertyChanged(nameof(Keyword));
                }
            }
        }

        public EnumItem<WorkerMessageChannel> SelectedChannel {
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
            get {
                bool value = true; ;
                if (NTMinerRoot.Instance.LocalAppSettingSet.TryGetAppSetting(nameof(IsErrorChecked), out IAppSetting setting) && setting.Value != null) {
                    value = (bool)setting.Value;
                }
                return value;
            }
            set {
                AppSettingData appSettingData = new AppSettingData() {
                    Key = nameof(IsErrorChecked),
                    Value = value
                };
                VirtualRoot.Execute(new ChangeLocalAppSettingCommand(appSettingData));
                OnPropertyChanged(nameof(IsErrorChecked));
                RefreshQueryResults();
            }
        }
        public bool IsWarnChecked {
            get {
                bool value = true; ;
                if (NTMinerRoot.Instance.LocalAppSettingSet.TryGetAppSetting(nameof(IsWarnChecked), out IAppSetting setting) && setting.Value != null) {
                    value = (bool)setting.Value;
                }
                return value;
            }
            set {
                AppSettingData appSettingData = new AppSettingData() {
                    Key = nameof(IsWarnChecked),
                    Value = value
                };
                VirtualRoot.Execute(new ChangeLocalAppSettingCommand(appSettingData));
                OnPropertyChanged(nameof(IsWarnChecked));
                RefreshQueryResults();
            }
        }
        public bool IsInfoChecked {
            get {
                bool value = true; ;
                if (NTMinerRoot.Instance.LocalAppSettingSet.TryGetAppSetting(nameof(IsInfoChecked), out IAppSetting setting) && setting.Value != null) {
                    value = (bool)setting.Value;
                }
                return value;
            }
            set {
                AppSettingData appSettingData = new AppSettingData() {
                    Key = nameof(IsInfoChecked),
                    Value = value
                };
                VirtualRoot.Execute(new ChangeLocalAppSettingCommand(appSettingData));
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

        public ObservableCollection<WorkerMessageViewModel> QueryResults {
            get {
                return _queyResults;
            }
        }

        private void RefreshQueryResults() {
            var query = _workerMessageVms.AsQueryable();
            if (SelectedChannel.Value != WorkerMessageChannel.Unspecified) {
                string channel = SelectedChannel.Value.GetName();
                query = query.Where(a => a.Channel == channel);
            }
            if (!IsErrorChecked || !IsWarnChecked || !IsInfoChecked) {
                query = query.Where(a => a.MessageTypeEnum == WorkerMessageType.Undefined
                    || (a.MessageTypeEnum == WorkerMessageType.Error && IsErrorChecked)
                    || (a.MessageTypeEnum == WorkerMessageType.Warn && IsWarnChecked)
                    || (a.MessageTypeEnum == WorkerMessageType.Info && IsInfoChecked));
            }
            _queyResults = new ObservableCollection<WorkerMessageViewModel>(query);
            OnPropertyChanged(nameof(QueryResults));
        }
    }
}
