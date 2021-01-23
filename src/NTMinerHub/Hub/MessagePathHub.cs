namespace NTMiner.Hub {
    using System;
    using System.Collections.Generic;
#if DEBUG
    using System.ComponentModel;
#endif
    using System.Linq;
    using System.Threading;

    public class MessagePathHub : IMessagePathHub {
        #region 内部类
        private interface IMessagePathSet {
            Type MessageType { get; }
            IEnumerable<IMessagePathId> GetMessagePaths();
            void RemoveMessagePath(IMessagePathId messagePathId);
        }

        private class MessagePathSet<TMessage> : IMessagePathSet {
            private readonly List<MessagePath<TMessage>> _messagePaths = new List<MessagePath<TMessage>>();
            private readonly object _locker = new object();
            public MessagePathSet() {
            }

            private readonly Type _messageType = typeof(TMessage);
            public Type MessageType {
                get {
                    return _messageType;
                }
            }

            public MessagePath<TMessage>[] GetMessagePaths() {
                return _messagePaths.ToArray();
            }

            IEnumerable<IMessagePathId> IMessagePathSet.GetMessagePaths() {
                return _messagePaths.ToArray();
            }

            public void AddMessagePath(MessagePath<TMessage> messagePath) {
                lock (_locker) {
                    if (typeof(ICmd).IsAssignableFrom(typeof(TMessage))) {
                        bool isExist = _messagePaths.Any(a => messagePath.PathId == a.PathId);
                        if (isExist) {
                            /// <see cref="ICmd"/>
                            throw new Exception($"一种命令只应被一个处理器处理:{typeof(TMessage).Name}");
                        }
                    }
                    else if (messagePath.Location != AnonymousMessagePath.Location && _messagePaths.Any(a => a.Path == messagePath.Path && a.PathId == messagePath.PathId)) {
                        NTMinerConsole.DevWarn(() => $"重复的路径:{messagePath.Path} {messagePath.Description}");
                    }
                    _messagePaths.Add(messagePath);
                }
            }

            public void RemoveMessagePath(IMessagePathId messagePathId) {
                lock (_locker) {
                    var item = _messagePaths.FirstOrDefault(a => ReferenceEquals(a, messagePathId));
                    if (item != null) {
                        _messagePaths.Remove(item);
                        NTMinerConsole.DevDebug(() => "拆除路径" + messagePathId.Path + messagePathId.Description);
                    }
                }
            }
        }

        private class MessagePathSetSet {
            private readonly Dictionary<Type, IMessagePathSet> _dicByMessageType = new Dictionary<Type, IMessagePathSet>();

            public MessagePathSetSet() { }

            private readonly object _locker = new object();
            public MessagePathSet<TMessage> GetMessagePathSet<TMessage>() {
                Type messageType = typeof(TMessage);
                if (_dicByMessageType.TryGetValue(messageType, out IMessagePathSet setSet)) {
                    return (MessagePathSet<TMessage>)setSet;
                }
                else {
                    lock (_locker) {
                        if (!_dicByMessageType.TryGetValue(messageType, out setSet)) {
                            MessagePathSet<TMessage> pathSet = new MessagePathSet<TMessage>();
                            _dicByMessageType.Add(messageType, pathSet);
                            return pathSet;
                        }
                        else {
                            return (MessagePathSet<TMessage>)setSet;
                        }
                    }
                }
            }

            public IEnumerable<IMessagePathId> GetAllMessagePathIds() {
                foreach (var set in _dicByMessageType.Values.ToArray()) {
                    foreach (var path in set.GetMessagePaths()) {
                        yield return path;
                    }
                }
            }

            public void RemoveMessagePath(IMessagePathId messagePathId) {
                if (_dicByMessageType.TryGetValue(messagePathId.MessageType, out IMessagePathSet set)) {
                    set.RemoveMessagePath(messagePathId);
                }
            }
        }

#if DEBUG
        public class MessagePath<TMessage> : IMessagePathId, INotifyPropertyChanged {
#else
    public class MessagePath<TMessage> : IMessagePathId {
#endif
            private readonly Action<TMessage> _path;
            private bool _isEnabled;
            private int _viaTimesLimit;

#if DEBUG
            public event PropertyChangedEventHandler PropertyChanged;
#endif

            internal MessagePath(Type location, string description, LogEnum logType, Action<TMessage> action, PathId pathId, int viaTimesLimit) {
                if (viaTimesLimit == 0) {
                    throw new InvalidProgramException("消息路径的viaTimesLimit不能为0，可以为负数表示不限制通过次数或为正数表示限定通过次数，但不能为0");
                }
                _isEnabled = true;
                MessageType = typeof(TMessage);
                Location = location;
                Path = $"{location.FullName}[{MessageType.FullName}]";
                Description = description;
                LogType = logType;
                _path = action;
                PathId = pathId;
                _viaTimesLimit = viaTimesLimit;
                CreatedOn = DateTime.Now;
            }

            public int ViaTimesLimit {
                get => _viaTimesLimit;
                private set {
                    _viaTimesLimit = value;
                }
            }

            internal void DecreaseViaTimesLimit(Action<IMessagePathId> onDownToZero) {
                int newValue = Interlocked.Decrement(ref _viaTimesLimit);
                if (newValue == 0) {
                    onDownToZero?.Invoke(this);
                }
#if DEBUG
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViaTimesLimit)));
#endif
            }

            public PathId PathId { get; private set; }
            public DateTime CreatedOn { get; private set; }
            public Type MessageType { get; private set; }
            public Type Location { get; private set; }
            public string Path { get; private set; }
            public LogEnum LogType { get; private set; }
            public string Description { get; private set; }
            public bool IsEnabled {
                get => _isEnabled;
                set {
                    _isEnabled = value;
#if DEBUG
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
#endif
                }
            }

            public void Go(TMessage message) {
                try {
                    _path?.Invoke(message);
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(Path + ":" + e.Message, e);
                }
            }
        }
        #endregion

        private readonly MessagePathSetSet PathSetSet = new MessagePathSetSet();
        public event Action<IMessagePathId> PathAdded;
        public event Action<IMessagePathId> PathRemoved;

        public MessagePathHub() {
        }

        #region IMessageDispatcher Members
        public IEnumerable<IMessagePathId> GetAllPaths() {
            foreach (var path in PathSetSet.GetAllMessagePathIds()) {
                yield return path;
            }
        }

        public void Route<TMessage>(TMessage message) where TMessage : IMessage {
            if (message == null) {
                throw new ArgumentNullException(nameof(message));
            }
            MessagePath<TMessage>[] messagePaths = PathSetSet.GetMessagePathSet<TMessage>().GetMessagePaths();
            if (messagePaths.Length == 0) {
                Type messageType = typeof(TMessage);
                MessageTypeAttribute messageTypeAttr = MessageTypeAttribute.GetMessageTypeAttribute(messageType);
                if (!messageTypeAttr.IsCanNoPath) {
                    NTMinerConsole.DevWarn(() => messageType.FullName + "类型的消息没有对应的处理器");
                }
            }
            else {
                foreach (var messagePath in messagePaths) {
                    try {
                        bool canGo = false;
                        if (message is IEvent evt) {
                            canGo =
                                evt.TargetPathId == PathId.Empty // 事件不是特定路径的事件则放行
                                || messagePath.PathId == PathId.Empty // 路径不是特定事件的路径则放行
                                || evt.TargetPathId == messagePath.PathId; // 路径是特定事件的路径且路径和事件造型放行
                        }
                        else if (message is ICmd cmd) {
                            // 路径不是特定命令的路径则放行
                            if (messagePath.PathId == PathId.Empty) {
                                canGo = true;
                            }
                            else {
                                canGo = messagePath.PathId == cmd.MessageId;
                            }
                        }
                        if (canGo && messagePath.ViaTimesLimit > 0) {
                            // ViaTimesLimite小于0表示是不限定通过的次数的路径，不限定通过的次数的路径不需要消息每通过一次递减一次ViaTimesLimit计数
                            messagePath.DecreaseViaTimesLimit(onDownToZero: RemovePath);
                        }
                        if (!messagePath.IsEnabled) {
                            continue;
                        }
                        if (canGo) {
                            switch (messagePath.LogType) {
                                case LogEnum.DevConsole:
                                    if (DevMode.IsDevMode) {
                                        NTMinerConsole.DevDebug(() => $"({typeof(TMessage).Name})->({messagePath.Location.Name}){messagePath.Description}");
                                    }
                                    break;
                                case LogEnum.UserConsole:
                                    NTMinerConsole.UserInfo($"({typeof(TMessage).Name})->({messagePath.Location.Name}){messagePath.Description}");
                                    break;
                                case LogEnum.Log:
                                    Logger.InfoDebugLine($"({typeof(TMessage).Name})->({messagePath.Location.Name}){messagePath.Description}");
                                    break;
                                case LogEnum.None:
                                default:
                                    break;
                            }
                            messagePath.Go(message);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                }
            }
        }

        public IMessagePathId AddPath<TMessage>(Type location, string description, LogEnum logType, Action<TMessage> action, PathId pathId, int viaTimesLimit = -1) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }
            MessagePath<TMessage> path = new MessagePath<TMessage>(location, description, logType, action, pathId, viaTimesLimit);
            PathSetSet.GetMessagePathSet<TMessage>().AddMessagePath(path);
            PathAdded?.Invoke(path);
            return path;
        }

        public void RemovePath(IMessagePathId pathId) {
            if (pathId == null) {
                return;
            }
            PathSetSet.RemoveMessagePath(pathId);
            PathRemoved?.Invoke(pathId);
        }
        #endregion
    }
}
