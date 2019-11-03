namespace NTMiner.Vms {
    using NTMiner.Core;
    using NTMiner.MinerServer;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public class WorkerMessagesViewModel : ViewModelBase {
        private ObservableCollection<WorkerMessageViewModel> _workerMessageVms;
        private ObservableCollection<WorkerMessageViewModel> _queyResults;
        private EnumItem<WorkerMessageChannel> _selectedChannel;
        private Dictionary<EnumItem<WorkerMessageChannel>, int> _errorCount = new Dictionary<EnumItem<WorkerMessageChannel>, int>();
        private Dictionary<EnumItem<WorkerMessageChannel>, int> _warnCount = new Dictionary<EnumItem<WorkerMessageChannel>, int>();
        private Dictionary<EnumItem<WorkerMessageChannel>, int> _infoCount = new Dictionary<EnumItem<WorkerMessageChannel>, int>();
        private string _keyword;

        public ICommand ClearKeyword { get; private set; }
        public ICommand Clear { get; private set; }

        public WorkerMessagesViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            foreach (var item in WorkerMessageChannel.Unspecified.GetEnumItems()) {
                _errorCount.Add(item, 0);
                _warnCount.Add(item, 0);
                _infoCount.Add(item, 0);
            }
            var data = VirtualRoot.WorkerMessages.Select(a => new WorkerMessageViewModel(a));
            _workerMessageVms = new ObservableCollection<WorkerMessageViewModel>(data);
            foreach (var item in _workerMessageVms) {
                switch (item.MessageTypeEnum) {
                    case WorkerMessageType.Error:
                        _errorCount[item.ChannelEnum.GetEnumItem()]++;
                        break;
                    case WorkerMessageType.Warn:
                        _warnCount[item.ChannelEnum.GetEnumItem()]++;
                        break;
                    case WorkerMessageType.Info:
                        _infoCount[item.ChannelEnum.GetEnumItem()]++;
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
                        foreach (var key in _errorCount.Keys) {
                            _errorCount[key] = 0;
                        }
                        foreach (var key in _warnCount.Keys) {
                            _warnCount[key] = 0;
                        }
                        foreach (var key in _infoCount.Keys) {
                            _infoCount[key] = 0;
                        }
                        CountChanged();
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
                                    _errorCount[vm.ChannelEnum.GetEnumItem()] += 1 - removedCount;
                                    OnPropertyChanged(nameof(ErrorCount));
                                    OnPropertyChanged(nameof(IsErrorCountVisible));
                                }
                                break;
                            case WorkerMessageType.Warn:
                                removedCount = message.Removes.Count(a => a.MessageType == WorkerMessageType.Warn.GetName());
                                if (removedCount != 1) {
                                    _warnCount[vm.ChannelEnum.GetEnumItem()] += 1 - removedCount;
                                    OnPropertyChanged(nameof(WarnCount));
                                    OnPropertyChanged(nameof(IsWarnCountVisible));
                                }
                                break;
                            case WorkerMessageType.Info:
                                removedCount = message.Removes.Count(a => a.MessageType == WorkerMessageType.Info.GetName());
                                if (removedCount != 1) {
                                    _infoCount[vm.ChannelEnum.GetEnumItem()] += 1 - removedCount;
                                    OnPropertyChanged(nameof(InfoCount));
                                    OnPropertyChanged(nameof(IsInfoCountVisible));
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
                    CountChanged();
                }
            }
        }

        private void CountChanged() {
            OnPropertyChanged(nameof(ErrorCount));
            OnPropertyChanged(nameof(WarnCount));
            OnPropertyChanged(nameof(InfoCount));
            OnPropertyChanged(nameof(IsErrorCountVisible));
            OnPropertyChanged(nameof(IsWarnCountVisible));
            OnPropertyChanged(nameof(IsInfoCountVisible));
        }

        public bool IsErrorChecked {
            get {
                return GetIsChecked(nameof(IsErrorChecked));
            }
            set {
                SetIsChecked(nameof(IsErrorChecked), value);
                OnPropertyChanged(nameof(IsErrorChecked));
                RefreshQueryResults();
            }
        }
        public bool IsWarnChecked {
            get {
                return GetIsChecked(nameof(IsWarnChecked));
            }
            set {
                SetIsChecked(nameof(IsWarnChecked), value);
                OnPropertyChanged(nameof(IsWarnChecked));
                RefreshQueryResults();
            }
        }
        public bool IsInfoChecked {
            get {
                return GetIsChecked(nameof(IsInfoChecked));
            }
            set {
                SetIsChecked(nameof(IsInfoChecked), value);
                OnPropertyChanged(nameof(IsInfoChecked));
                RefreshQueryResults();
            }
        }

        private bool GetIsChecked(string key) {
            bool value = true; ;
            if (NTMinerRoot.Instance.LocalAppSettingSet.TryGetAppSetting(key, out IAppSetting setting) && setting.Value != null) {
                value = (bool)setting.Value;
            }
            return value;
        }

        private void SetIsChecked(string key, bool value) {
            AppSettingData appSettingData = new AppSettingData() {
                Key = key,
                Value = value
            };
            VirtualRoot.Execute(new SetLocalAppSettingCommand(appSettingData));
        }

        public int ErrorCount {
            get {
                if (SelectedChannel.Value == WorkerMessageChannel.Unspecified) {
                    return _errorCount.Where(a => a.Key.Value != WorkerMessageChannel.Unspecified).Sum(a => a.Value);
                }
                return _errorCount[SelectedChannel];
            }
        }
        public Visibility IsErrorCountVisible {
            get {
                if (ErrorCount > 0) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public int WarnCount {
            get {
                if (SelectedChannel.Value == WorkerMessageChannel.Unspecified) {
                    return _warnCount.Where(a => a.Key.Value != WorkerMessageChannel.Unspecified).Sum(a => a.Value);
                }
                return _warnCount[SelectedChannel];
            }
        }
        public Visibility IsWarnCountVisible {
            get {
                if (WarnCount > 0) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public int InfoCount {
            get {
                if (SelectedChannel.Value == WorkerMessageChannel.Unspecified) {
                    return _infoCount.Where(a => a.Key.Value != WorkerMessageChannel.Unspecified).Sum(a => a.Value);
                }
                return _infoCount[SelectedChannel];
            }
        }
        public Visibility IsInfoCountVisible {
            get {
                if (InfoCount > 0) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public ObservableCollection<WorkerMessageViewModel> QueryResults {
            get {
                return _queyResults;
            }
        }

        private bool IsCheckedAllMessageType {
            get {
                return IsErrorChecked && IsWarnChecked && IsInfoChecked;
            }
        }

        private void RefreshQueryResults() {
            if (SelectedChannel.Value == WorkerMessageChannel.Unspecified && IsCheckedAllMessageType && string.IsNullOrEmpty(Keyword)) {
                _queyResults = _workerMessageVms;
                OnPropertyChanged(nameof(QueryResults));
                return;
            }
            var query = _workerMessageVms.AsQueryable();
            if (SelectedChannel.Value != WorkerMessageChannel.Unspecified) {
                query = query.Where(a => a.Channel == SelectedChannel.Value.GetName());
            }
            if (!IsCheckedAllMessageType) {
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
