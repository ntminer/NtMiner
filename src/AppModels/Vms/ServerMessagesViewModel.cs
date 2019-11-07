using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ServerMessagesViewModel : ViewModelBase {
        private ObservableCollection<ServerMessageViewModel> _serverMessageVms;
        private ObservableCollection<ServerMessageViewModel> _queyResults;
        private string _keyword;
        private readonly Dictionary<ServerMessageType, MessageTypeItem<ServerMessageType>> _count = new Dictionary<ServerMessageType, MessageTypeItem<ServerMessageType>>();

        public ICommand Add { get; private set; }

        public ICommand ClearKeyword { get; private set; }
        public ICommand Clear { get; private set; }

        public ServerMessagesViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            foreach (var messageType in EnumItem<ServerMessageType>.GetEnumItems()) {
                _count.Add(messageType.Value, new MessageTypeItem<ServerMessageType>(messageType, ServerMessageViewModel.GetIcon, ServerMessageViewModel.GetIconFill, RefreshQueryResults));
            }
            Init();
            RefreshQueryResults();
            this.Add = new DelegateCommand(() => {
                new ServerMessageViewModel(new ServerMessageData {
                    Id = Guid.NewGuid(),
                    MessageType = ServerMessageType.Info.GetName(),
                    Provider = "admin",
                    Content = string.Empty,
                    Timestamp = DateTime.MinValue
                }).Edit.Execute(FormType.Add);
            });
            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
            this.Clear = new DelegateCommand(() => {
                this.ShowDialog(new DialogWindowViewModel(message: "确定清空吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new ClearServerMessages());
                }));
            });
            VirtualRoot.BuildEventPath<ServerMessagesClearedEvent>("清空了本地存储的服务器消息后刷新Vm内存", LogEnum.DevConsole,
                action: message => {
                    bool needInitQueryResuts = _queyResults != _serverMessageVms;
                    Init();
                    if (needInitQueryResuts) {
                        _queyResults = new ObservableCollection<ServerMessageViewModel>();
                        foreach (var item in _serverMessageVms) {
                            if (_count[item.MessageTypeEnum].IsChecked) {
                                _queyResults.Add(item);
                            }
                        }
                        OnPropertyChanged(nameof(QueryResults));
                    }
                });
            VirtualRoot.BuildEventPath<NewServerMessageLoadedEvent>("从服务器加载了新消息后刷新Vm内存", LogEnum.DevConsole,
                action: message => {
                    foreach (var item in message.Data) {
                        _serverMessageVms.Add(new ServerMessageViewModel(item));
                    }
                });
            _serverMessageVms = new ObservableCollection<ServerMessageViewModel>(NTMinerRoot.Instance.LocalServerMessageSet.Select(a => new ServerMessageViewModel(a)));
        }

        private void Init() {
            var data = NTMinerRoot.Instance.LocalServerMessageSet.Select(a => new ServerMessageViewModel(a));
            _serverMessageVms = new ObservableCollection<ServerMessageViewModel>(data);
            foreach (var key in _count.Keys) {
                _count[key].Count = 0;
            }
            foreach (var item in _serverMessageVms) {
                _count[item.MessageTypeEnum].Count++;
            }
        }

        public IEnumerable<MessageTypeItem<ServerMessageType>> MessageTypeItems {
            get {
                return _count.Values;
            }
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

        public ObservableCollection<ServerMessageViewModel> ServerMessageVms {
            get {
                return _serverMessageVms;
            }
        }
        public ObservableCollection<ServerMessageViewModel> QueryResults {
            get {
                return _queyResults;
            }
        }

        private void RefreshQueryResults() {
            bool isCheckedAllMessageType = _count.Values.All(a => a.IsChecked);
            if (isCheckedAllMessageType && string.IsNullOrEmpty(Keyword)) {
                _queyResults = _serverMessageVms;
                OnPropertyChanged(nameof(QueryResults));
                return;
            }
            var query = _serverMessageVms.AsQueryable();
            if (!isCheckedAllMessageType) {
                query = query.Where(a => _count[a.MessageTypeEnum].IsChecked);
            }
            if (!string.IsNullOrEmpty(Keyword)) {
                query = query.Where(a => a.Content != null && a.Content.Contains(Keyword));
            }
            _queyResults = new ObservableCollection<ServerMessageViewModel>(query);
            OnPropertyChanged(nameof(QueryResults));
        }
    }
}
