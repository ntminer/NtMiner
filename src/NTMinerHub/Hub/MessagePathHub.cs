namespace NTMiner.Hub {
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
                    else if (messagePath.Location != MessagePath.Anonymous && _messagePaths.Any(a => a.Path == messagePath.Path && a.PathId == messagePath.PathId)) {
                        Write.DevWarn(() => $"重复的路径:{messagePath.Path} {messagePath.Description}");
                    }
                    _messagePaths.Add(messagePath);
                }
            }

            public void RemoveMessagePath(IMessagePathId messagePathId) {
                lock (_locker) {
                    var item = _messagePaths.FirstOrDefault(a => ReferenceEquals(a, messagePathId));
                    if (item != null) {
                        _messagePaths.Remove(item);
                        Write.DevDebug(() => "拆除路径" + messagePathId.Path + messagePathId.Description);
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
                    Write.DevWarn(() => messageType.FullName + "类型的消息没有对应的处理器");
                }
            }
            else {
                foreach (var messagePath in messagePaths) {
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
                                    Write.DevDebug(() => $"({typeof(TMessage).Name})->({messagePath.Location.Name}){messagePath.Description}");
                                }
                                break;
                            case LogEnum.UserConsole:
                                Write.UserInfo($"({typeof(TMessage).Name})->({messagePath.Location.Name}){messagePath.Description}");
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
            }
        }

        public void AddPath<TMessage>(MessagePath<TMessage> path) {
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }
            PathSetSet.GetMessagePathSet<TMessage>().AddMessagePath(path);
            PathAdded?.Invoke(path);
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
