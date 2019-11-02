namespace NTMiner.Vms {
    using NTMiner.Core;
    using NTMiner.MinerServer;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    public class WorkerMessagesViewModel : ViewModelBase {
        private ObservableCollection<WorkerMessageViewModel> _workerMessageVms;
        private ObservableCollection<WorkerMessageViewModel> _queyResults;
        private EnumItem<WorkerMessageChannel> _selectedChannel;
        private int _errorCount;
        private int _warnCount;
        private int _infoCount;
        private string _keyword;

        public ICommand ClearKeyword { get; private set; }
        public ICommand Clear { get; private set; }

        public WorkerMessagesViewModel() {
            var data = VirtualRoot.WorkerMessages.Select(a => new WorkerMessageViewModel(a));
            _workerMessageVms = new ObservableCollection<WorkerMessageViewModel>(data);
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
            RefreshQueryResults();

            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
            this.Clear = new DelegateCommand(() => {
                this.ShowDialog(new DialogWindowViewModel(message: "确定清空吗？", title: "确认", onYes: () => {
                    VirtualRoot.WorkerMessages.Clear();
                }));
            });
            VirtualRoot.BuildEventPath<WorkerMessageClearedEvent>("清空挖矿消息集后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        _workerMessageVms = new ObservableCollection<WorkerMessageViewModel>();
                        _queyResults = new ObservableCollection<WorkerMessageViewModel>();
                        OnPropertyChanged(nameof(QueryResults));
                        ErrorCount = 0;
                        WarnCount = 0;
                        InfoCount = 0;
                    });
                });
            VirtualRoot.BuildEventPath<WorkerMessageAddedEvent>("发生了挖矿事件后刷新Vm内存", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        var vm = new WorkerMessageViewModel(message.Source);
                        _workerMessageVms.Insert(0, vm);
                        foreach (var item in message.Removes) {
                            var toRemove = _workerMessageVms.FirstOrDefault(a => a.Id == item.Id);
                            if (toRemove != null) {
                                _workerMessageVms.Remove(toRemove);
                            }
                        }
                        int removedCount = 0;
                        switch (vm.MessageTypeEnum) {
                            case WorkerMessageType.Error:
                                removedCount = message.Removes.Count(a => a.MessageType == WorkerMessageType.Error.GetName());
                                if (removedCount != 1) {
                                    ErrorCount += 1 - removedCount;
                                }
                                break;
                            case WorkerMessageType.Warn:
                                removedCount = message.Removes.Count(a => a.MessageType == WorkerMessageType.Warn.GetName());
                                if (removedCount != 1) {
                                    WarnCount += 1 - removedCount;
                                }
                                break;
                            case WorkerMessageType.Info:
                                removedCount = message.Removes.Count(a => a.MessageType == WorkerMessageType.Info.GetName());
                                if (removedCount != 1) {
                                    InfoCount += 1 - removedCount;
                                }
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
                });
        }

        public string Keyword {
            get => _keyword;
            set {
                if (_keyword != value) {
                    _keyword = value;
                    OnPropertyChanged(nameof(Keyword));
                    RefreshQueryResults();
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
                VirtualRoot.Execute(new SetLocalAppSettingCommand(appSettingData));
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
                VirtualRoot.Execute(new SetLocalAppSettingCommand(appSettingData));
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
                VirtualRoot.Execute(new SetLocalAppSettingCommand(appSettingData));
                OnPropertyChanged(nameof(IsInfoChecked));
                RefreshQueryResults();
            }
        }

        public int ErrorCount {
            get => _errorCount;
            set {
                if (_errorCount != value) {
                    _errorCount = value;
                    OnPropertyChanged(nameof(ErrorCount));
                }
            }
        }
        public int WarnCount {
            get => _warnCount;
            set {
                if (_warnCount != value) {
                    _warnCount = value;
                    OnPropertyChanged(nameof(WarnCount));
                }
            }
        }
        public int InfoCount {
            get => _infoCount;
            set {
                if (_infoCount != value) {
                    _infoCount = value;
                    OnPropertyChanged(nameof(InfoCount));
                }
            }
        }

        public ObservableCollection<WorkerMessageViewModel> QueryResults {
            get {
                return _queyResults;
            }
        }

        private void RefreshQueryResults() {
            if (SelectedChannel.Value == WorkerMessageChannel.Unspecified && IsErrorChecked && IsWarnChecked && IsInfoChecked && string.IsNullOrEmpty(Keyword)) {
                _queyResults = _workerMessageVms;
                OnPropertyChanged(nameof(QueryResults));
                return;
            }
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
            if (!string.IsNullOrEmpty(Keyword)) {
                query = query.Where(a => a.Content != null && a.Content.Contains(Keyword));
            }
            _queyResults = new ObservableCollection<WorkerMessageViewModel>(query);
            OnPropertyChanged(nameof(QueryResults));
        }
    }
}
