namespace NTMiner.Vms {
    using NTMiner.MinerClient;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    public class LocalMessagesViewModel : ViewModelBase {
        private ObservableCollection<LocalMessageViewModel> _localMessageVms;
        private ObservableCollection<LocalMessageViewModel> _queyResults;
        private EnumItem<LocalMessageChannel> _selectedChannel;
        private string _keyword;
        private readonly Dictionary<EnumItem<LocalMessageChannel>, Dictionary<LocalMessageType, MessageTypeItem<LocalMessageType>>> _count = new Dictionary<EnumItem<LocalMessageChannel>, Dictionary<LocalMessageType, MessageTypeItem<LocalMessageType>>>();

        private void UpdateChannelAll() {
            var channelAll = LocalMessageChannel.Unspecified.GetEnumItem();
            if (!_count.ContainsKey(channelAll)) {
                return;
            }
            var dic = _count[channelAll];
            foreach (var key in dic.Keys) {
                dic[key].Count = _count.Where(a => a.Key != channelAll).Sum(a => a.Value[key].Count);
            }
        }


        public IEnumerable<EnumItem<LocalMessageChannel>> LocalMessageChannelEnumItems {
            get {
                // 只有挖矿端有This和Kernel两个频道
                if (VirtualRoot.IsMinerClient) {
                    foreach (var item in NTMinerRoot.LocalMessageChannelEnumItems) {
                        yield return item;
                    }
                }
                else {
                    yield return LocalMessageChannel.This.GetEnumItem();
                }
            }
        }

        public IEnumerable<MessageTypeItem<LocalMessageType>> MessageTypeItems {
            get {
                Dictionary<LocalMessageType, MessageTypeItem<LocalMessageType>> values = _count[SelectedChannel];
                return values.Values;
            }
        }

        public ICommand ClearKeyword { get; private set; }
        public ICommand Clear { get; private set; }

        public LocalMessagesViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            foreach (var messageChannel in LocalMessageChannelEnumItems) {
                var values = new Dictionary<LocalMessageType, MessageTypeItem<LocalMessageType>>();
                foreach (var messageType in NTMinerRoot.LocalMessageTypeEnumItems) {
                    values.Add(messageType.Value, new MessageTypeItem<LocalMessageType>(messageType, LocalMessageViewModel.GetIcon, LocalMessageViewModel.GetIconFill, RefreshQueryResults));
                }
                _count.Add(messageChannel, values);
            }
            _selectedChannel = LocalMessageChannelEnumItems.FirstOrDefault();
            Init();

            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
            this.Clear = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: "确定清空吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new ClearLocalMessageSetCommand());
                }));
            });
            VirtualRoot.BuildEventPath<LocalMessageSetClearedEvent>("清空挖矿消息集后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Init();
                    });
                });
            VirtualRoot.BuildEventPath<LocalMessageAddedEvent>("发生了挖矿事件后刷新Vm内存", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        var vm = new LocalMessageViewModel(message.Target);
                        _localMessageVms.Insert(0, vm);
                        if (IsSatisfyQuery(vm)) {
                            _queyResults.Insert(0, vm);
                        }
                        foreach (var item in message.Removes) {
                            var toRemove = _localMessageVms.FirstOrDefault(a => a.Id == item.Id);
                            if (toRemove != null) {
                                _localMessageVms.Remove(toRemove);
                                if (IsSatisfyQuery(toRemove)) {
                                    _queyResults.Remove(toRemove);
                                }
                            }
                        }
                        int removedCount = message.Removes.Count(a => a.MessageType == vm.MessageTypeEnum.GetName());
                        if (removedCount != 1) {
                            _count[vm.ChannelEnum.GetEnumItem()][vm.MessageTypeEnum].Count += 1 - removedCount;
                            UpdateChannelAll();
                        }
                        OnPropertyChanged(nameof(IsNoRecord));
                    });
                });
            VirtualRoot.BuildEventPath<NewDayEvent>("新的一天到来时刷新消息集中的可读性时间戳展示", LogEnum.DevConsole,
                action: message => {
                    if (QueryResults == null) {
                        return;
                    }
                    foreach (var item in QueryResults) {
                        if (item.Timestamp.Date.AddDays(3) >= message.Timestamp.Date) {
                            item.OnPropertyChanged(nameof(item.TimestampText));
                        }
                        else {
                            // 因为是按照时间倒叙排列的，所以可以break
                            break;
                        }
                    }
                });
        }

        private void Init() {
            var data = VirtualRoot.LocalMessages.AsEnumerable().Select(a => new LocalMessageViewModel(a));
            _localMessageVms = new ObservableCollection<LocalMessageViewModel>(data);
            foreach (var dic in _count.Values) {
                foreach (var key in dic.Keys) {
                    dic[key].Count = 0;
                }
            }
            foreach (var item in _localMessageVms) {
                _count[item.ChannelEnum.GetEnumItem()][item.MessageTypeEnum].Count++;
            }
            RefreshQueryResults();
            UpdateChannelAll();
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

        public EnumItem<LocalMessageChannel> SelectedChannel {
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

        public ObservableCollection<LocalMessageViewModel> QueryResults {
            get {
                return _queyResults;
            }
        }

        private bool IsSatisfyQuery(LocalMessageViewModel vm) {
            if (_queyResults == _localMessageVms) {
                return false;
            }
            if (_count[SelectedChannel][vm.MessageTypeEnum].IsChecked && (string.IsNullOrEmpty(Keyword) || vm.Content.Contains(Keyword))) {
                return true;
            }
            return false;
        }

        public bool IsNoRecord {
            get {
                return _queyResults.Count == 0;
            }
        }

        private void RefreshQueryResults() {
            bool isCheckedAllMessageType = _count[SelectedChannel].Values.All(a => a.IsChecked);
            if (SelectedChannel.Value == LocalMessageChannel.Unspecified && isCheckedAllMessageType && string.IsNullOrEmpty(Keyword)) {
                if (_queyResults != _localMessageVms) {
                    _queyResults = _localMessageVms;
                    OnPropertyChanged(nameof(IsNoRecord));
                    OnPropertyChanged(nameof(QueryResults));
                }
                return;
            }
            var query = _localMessageVms.AsQueryable();
            if (SelectedChannel.Value != LocalMessageChannel.Unspecified) {
                query = query.Where(a => a.Channel == SelectedChannel.Value.GetName());
            }
            if (!isCheckedAllMessageType) {
                query = query.Where(a => _count[SelectedChannel][a.MessageTypeEnum].IsChecked);
            }
            if (!string.IsNullOrEmpty(Keyword)) {
                query = query.Where(a => a.Content != null && a.Content.Contains(Keyword));
            }
            _queyResults = new ObservableCollection<LocalMessageViewModel>(query);
            OnPropertyChanged(nameof(IsNoRecord));
            OnPropertyChanged(nameof(QueryResults));
        }
    }
}
