namespace NTMiner.Vms {
    using NTMiner.MinerClient;
    using NTMiner.MinerServer;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Media;

    public class WorkerMessagesViewModel : ViewModelBase {
        #region MessageTypeItem class
        public class MessageTypeItem : ViewModelBase {
            private int _count;
            private readonly Action OnIsCheckedChanged;

            public MessageTypeItem(EnumItem<WorkerMessageType> messageType, Action onIsCheckedChanged) {
                this.MessageType = messageType;
                this.OnIsCheckedChanged = onIsCheckedChanged;
            }
            public EnumItem<WorkerMessageType> MessageType { get; private set; }
            public StreamGeometry Icon {
                get {
                    return WorkerMessageViewModel.GetIcon(MessageType.Value);
                }
            }
            public SolidColorBrush IconFill {
                get {
                    return WorkerMessageViewModel.GetIconFill(MessageType.Value);
                }
            }
            public string DisplayText {
                get {
                    return MessageType.Description;
                }
            }

            public bool IsChecked {
                get {
                    bool value = true;
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
                    OnIsCheckedChanged?.Invoke();
                }
            }

            public int Count {
                get => _count;
                set {
                    _count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }
        #endregion

        private ObservableCollection<WorkerMessageViewModel> _workerMessageVms;
        private ObservableCollection<WorkerMessageViewModel> _queyResults;
        private EnumItem<WorkerMessageChannel> _selectedChannel;
        private readonly Dictionary<EnumItem<WorkerMessageChannel>, Dictionary<WorkerMessageType, MessageTypeItem>> _count = new Dictionary<EnumItem<WorkerMessageChannel>, Dictionary<WorkerMessageType, MessageTypeItem>>();
        private string _keyword;

        private void UpdateChannelAll() {
            var channelAll = WorkerMessageChannel.Unspecified.GetEnumItem();
            var dic = _count[channelAll];
            foreach (var key in dic.Keys) {
                dic[key].Count = _count.Where(a => a.Key != channelAll).Sum(a => a.Value[key].Count);
            }
        }


        public IEnumerable<EnumItem<WorkerMessageChannel>> WorkerMessageChannelEnumItems {
            get {
                return WorkerMessageChannel.Unspecified.GetEnumItems();
            }
        }

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
                foreach (var messageType in WorkerMessageType.Info.GetEnumItems()) {
                    values.Add(messageType.Value, new MessageTypeItem(messageType, RefreshQueryResults));
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
            UpdateChannelAll();

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
                        OnPropertyChanged(nameof(MessageTypeItems));
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
                            UpdateChannelAll();
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
                    OnPropertyChanged(nameof(MessageTypeItems));
                }
            }
        }

        public ObservableCollection<WorkerMessageViewModel> QueryResults {
            get {
                return _queyResults;
            }
        }

        private void RefreshQueryResults() {
            bool isCheckedAllMessageType = _count[SelectedChannel].Values.All(a => a.IsChecked);
            if (SelectedChannel.Value == WorkerMessageChannel.Unspecified && isCheckedAllMessageType && string.IsNullOrEmpty(Keyword)) {
                _queyResults = _workerMessageVms;
                OnPropertyChanged(nameof(QueryResults));
                return;
            }
            var query = _workerMessageVms.AsQueryable();
            if (SelectedChannel.Value != WorkerMessageChannel.Unspecified) {
                query = query.Where(a => a.Channel == SelectedChannel.Value.GetName());
            }
            if (!isCheckedAllMessageType) {
                query = query.Where(a => _count[SelectedChannel][a.MessageTypeEnum].IsChecked);
            }
            if (!string.IsNullOrEmpty(Keyword)) {
                query = query.Where(a => a.Content != null && a.Content.Contains(Keyword));
            }
            _queyResults = new ObservableCollection<WorkerMessageViewModel>(query);
            OnPropertyChanged(nameof(QueryResults));
        }
    }
}
