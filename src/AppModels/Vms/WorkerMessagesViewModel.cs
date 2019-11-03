namespace NTMiner.Vms {
    using NTMiner.Core;
    using NTMiner.MinerServer;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public class WorkerMessagesViewModel : ViewModelBase {
        public class MessageTypeItem : ViewModelBase {
            private int _count;

            public MessageTypeItem(EnumItem<WorkerMessageType> messageType) {
                this.MessageType = messageType;
            }
            public EnumItem<WorkerMessageType> MessageType { get; private set; }
            public string Icon {
                get {
                    return $"Icon_{MessageType.Name}";
                }
            }
            public string DisplayText {
                get {
                    return MessageType.Description;
                }
            }

            public bool IsChecked {
                get {
                    bool value = true; ;
                    if (NTMinerRoot.Instance.LocalAppSettingSet.TryGetAppSetting($"Is{MessageType.Name}Checked", out IAppSetting setting) && setting.Value != null) {
                        value = (bool)setting.Value;
                    }
                    return value;
                }
                set {
                    AppSettingData appSettingData = new AppSettingData() {
                        Key = $"Is{MessageType.Name}Checked",
                        Value = value
                    };
                    VirtualRoot.Execute(new SetLocalAppSettingCommand(appSettingData));
                    OnPropertyChanged(nameof(IsChecked));
                }
            }

            public int Count {
                get => _count;
                set {
                    _count = value;
                    OnPropertyChanged(nameof(Count));
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
            public Visibility IsVisible {
                get {
                    if (Count > 0) {
                        return Visibility.Visible;
                    }
                    return Visibility.Collapsed;
                }
            }
        }

        private ObservableCollection<WorkerMessageViewModel> _workerMessageVms;
        private ObservableCollection<WorkerMessageViewModel> _queyResults;
        private EnumItem<WorkerMessageChannel> _selectedChannel;
        private readonly Dictionary<EnumItem<WorkerMessageChannel>, Dictionary<WorkerMessageType, MessageTypeItem>> _count = new Dictionary<EnumItem<WorkerMessageChannel>, Dictionary<WorkerMessageType, MessageTypeItem>>();
        private string _keyword;

        public IEnumerable<MessageTypeItem> MessageTypeItems {
            get {
                Dictionary<WorkerMessageType, MessageTypeItem> values = _count[SelectedChannel];
                return values.Values;
            }
        }

        public ICommand ClearKeyword { get; private set; }
        public ICommand Clear { get; private set; }

        public WorkerMessagesViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            foreach (var messageChannel in WorkerMessageChannel.Unspecified.GetEnumItems()) {
                var values = new Dictionary<WorkerMessageType, MessageTypeItem>();
                foreach (var messageType in WorkerMessageType.Undefined.GetEnumItems()) {
                    values.Add(messageType.Value, new MessageTypeItem(messageType));
                }
                _count.Add(messageChannel, values);
            }
            var data = VirtualRoot.WorkerMessages.Select(a => new WorkerMessageViewModel(a));
            _workerMessageVms = new ObservableCollection<WorkerMessageViewModel>(data);
            foreach (var item in _workerMessageVms) {
                _count[item.ChannelEnum.GetEnumItem()][item.MessageTypeEnum].Count++;
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
                        foreach (var item in _count.Values) {
                            foreach (var key in item.Keys) {
                                item[key].Count = 0;
                            }
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
                        int removedCount = message.Removes.Count(a => a.MessageType == vm.MessageTypeEnum.GetName());
                        if (removedCount != 1) {
                            _count[vm.ChannelEnum.GetEnumItem()][vm.MessageTypeEnum].Count += 1 - removedCount;
                            OnPropertyChanged($"{vm.MessageTypeEnum.GetName()}Count");
                            OnPropertyChanged($"Is{vm.MessageTypeEnum.GetName()}CountVisible");
                        }

                        if (_queyResults != _workerMessageVms) {
                            #region 更新QueryResults
                            bool isSelectedChannel = SelectedChannel.Value == vm.ChannelEnum;
                            bool isMessageTypeChecked = false;
                            var isCheckedProperty = this.GetType().GetProperty($"Is{vm.MessageTypeEnum.GetName()}Checked");
                            if (isCheckedProperty != null) {
                                isMessageTypeChecked = (bool)isCheckedProperty.GetValue(this, null);
                            }
                            if (isSelectedChannel && isMessageTypeChecked) {
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
                    return _count.Where(a => a.Key.Value != WorkerMessageChannel.Unspecified).Sum(a => a.Value[WorkerMessageType.Error].Count);
                }
                return _count[SelectedChannel][WorkerMessageType.Error].Count;
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
                    return _count.Where(a => a.Key.Value != WorkerMessageChannel.Unspecified).Sum(a => a.Value[WorkerMessageType.Warn].Count);
                }
                return _count[SelectedChannel][WorkerMessageType.Warn].Count;
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
                    return _count.Where(a => a.Key.Value != WorkerMessageChannel.Unspecified).Sum(a => a.Value[WorkerMessageType.Info].Count);
                }
                return _count[SelectedChannel][WorkerMessageType.Info].Count;
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
