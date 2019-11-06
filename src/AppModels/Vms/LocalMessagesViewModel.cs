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
                    foreach (var item in EnumItem<LocalMessageChannel>.GetEnumItems()) {
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
                foreach (var messageType in EnumItem<LocalMessageType>.GetEnumItems()) {
                    values.Add(messageType.Value, new MessageTypeItem<LocalMessageType>(messageType, LocalMessageViewModel.GetIcon, LocalMessageViewModel.GetIconFill, RefreshQueryResults));
                }
                _count.Add(messageChannel, values);
            }
            var data = VirtualRoot.LocalMessages.Select(a => new LocalMessageViewModel(a));
            _localMessageVms = new ObservableCollection<LocalMessageViewModel>(data);
            foreach (var item in _localMessageVms) {
                _count[item.ChannelEnum.GetEnumItem()][item.MessageTypeEnum].Count++;
            }
            _selectedChannel = LocalMessageChannelEnumItems.FirstOrDefault();
            RefreshQueryResults();
            UpdateChannelAll();

            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
            this.Clear = new DelegateCommand(() => {
                this.ShowDialog(new DialogWindowViewModel(message: "确定清空吗？", title: "确认", onYes: () => {
                    VirtualRoot.LocalMessages.Clear();
                }));
            });
            VirtualRoot.BuildEventPath<LocalMessageClearedEvent>("清空挖矿消息集后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        _localMessageVms = new ObservableCollection<LocalMessageViewModel>();
                        foreach (var item in _count.Values) {
                            foreach (var key in item.Keys) {
                                item[key].Count = 0;
                            }
                        }
                        RefreshQueryResults();
                    });
                });
            VirtualRoot.BuildEventPath<LocalMessageAddedEvent>("发生了挖矿事件后刷新Vm内存", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        var vm = new LocalMessageViewModel(message.Source);
                        _localMessageVms.Insert(0, vm);
                        foreach (var item in message.Removes) {
                            var toRemove = _localMessageVms.FirstOrDefault(a => a.Id == item.Id);
                            if (toRemove != null) {
                                _localMessageVms.Remove(toRemove);
                            }
                        }
                        int removedCount = message.Removes.Count(a => a.MessageType == vm.MessageTypeEnum.GetName());
                        if (removedCount != 1) {
                            _count[vm.ChannelEnum.GetEnumItem()][vm.MessageTypeEnum].Count += 1 - removedCount;
                            UpdateChannelAll();
                            OnPropertyChanged($"{vm.MessageTypeEnum.GetName()}Count");
                            OnPropertyChanged($"Is{vm.MessageTypeEnum.GetName()}CountVisible");
                        }

                        if (_queyResults != _localMessageVms) {
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

        private void RefreshQueryResults() {
            bool isCheckedAllMessageType = _count[SelectedChannel].Values.All(a => a.IsChecked);
            if (SelectedChannel.Value == LocalMessageChannel.Unspecified && isCheckedAllMessageType && string.IsNullOrEmpty(Keyword)) {
                _queyResults = _localMessageVms;
                OnPropertyChanged(nameof(QueryResults));
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
            OnPropertyChanged(nameof(QueryResults));
        }
    }
}
