namespace NTMiner.Hub {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class MessageHub : IMessageHub {
        #region 内部类
        private interface IMessagePathSet {
            Type MessageType { get; }
            IEnumerable<IMessagePathId> GetMessagePaths();
            void RemoveMessagePath(IMessagePathId messagePathId);
        }

        private class MessagePathSet<TMessage> : IMessagePathSet {
            public static readonly MessagePathSet<TMessage> Instance = new MessagePathSet<TMessage>();

            private readonly List<MessagePath<TMessage>> _messagePaths = new List<MessagePath<TMessage>>();
            private readonly object _locker = new object();

            private MessagePathSet() {
                MessagePathSetSet.Add(this);
            }

            public Type MessageType {
                get {
                    return typeof(TMessage);
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
                        if (_messagePaths.Any(a => a.Path == messagePath.Path && messagePath.PathId == a.PathId)) {
                            // 因为一种命令只应被一个处理器处理，命令实际上可以设计为不走总线，
                            // 之所以设计为统一走总线只是为了通过将命令类型集中表达以起文档作用。
                            throw new Exception($"一种命令只应被一个处理器处理:{typeof(TMessage).Name}");
                        }
                    }
                    else if (_messagePaths.Any(a => a.Path == messagePath.Path && a.PathId == messagePath.PathId)) {
                        Write.DevWarn($"重复的路径:{messagePath.Path} {messagePath.Description}");
                    }
                    _messagePaths.Add(messagePath);
                }
            }

            public void RemoveMessagePath(IMessagePathId messagePathId) {
                lock (_locker) {
                    var item = _messagePaths.FirstOrDefault(a => ReferenceEquals(a, messagePathId));
                    if (item != null) {
                        _messagePaths.Remove(item);
                        Write.DevDebug("拆除路径" + messagePathId.Path + messagePathId.Description);
                    }
                }
            }
        }

        private static class MessagePathSetSet {
            private static readonly Dictionary<Type, IMessagePathSet> _dicByMessageType = new Dictionary<Type, IMessagePathSet>();

            public static void Add(IMessagePathSet messagePathSet) {
                _dicByMessageType.Add(messagePathSet.MessageType, messagePathSet);
            }

            public static IEnumerable<IMessagePathId> GetAllMessagePathIds() {
                foreach (var set in _dicByMessageType.Values.ToArray()) {
                    foreach (var path in set.GetMessagePaths()) {
                        yield return path;
                    }
                }
            }

            public static void RemoveMessagePath(IMessagePathId messagePathId) {
                if (_dicByMessageType.TryGetValue(messagePathId.MessageType, out IMessagePathSet set)) {
                    set.RemoveMessagePath(messagePathId);
                }
            }
        }
        #endregion

        public event Action<IMessagePathId> MessagePathAdded;
        public event Action<IMessagePathId> MessagePathRemoved;

        public MessageHub() {
        }

        #region IMessageDispatcher Members
        public IEnumerable<IMessagePathId> GetAllPaths() {
            foreach (var path in MessagePathSetSet.GetAllMessagePathIds()) {
                yield return path;
            }
        }

        private static readonly Type _per1SecondMessageType = typeof(Per1SecondEvent);
        public void Route<TMessage>(TMessage message) where TMessage : IMessage {
            if (message == null) {
                throw new ArgumentNullException(nameof(message));
            }
            MessagePath<TMessage>[] messagePaths = MessagePathSet<TMessage>.Instance.GetMessagePaths();
            Type messageType = typeof(TMessage);
            if (messagePaths.Length == 0) {
                MessageTypeAttribute messageTypeAttr = MessageTypeAttribute.GetMessageTypeAttribute(messageType);
                if (!messageTypeAttr.IsCanNoHandler) {
                    Write.DevWarn(messageType.FullName + "类型的消息没有对应的处理器");
                }
            }
            else {
                bool isPer1SecondMessage = messageType == _per1SecondMessageType;
                if (isPer1SecondMessage) {
                    foreach (dynamic messagePath in MessagePathSetSet.GetAllMessagePathIds()) {
                        if (messagePath.LifeLimitSeconds > 0) {
                            dynamic newValue = messagePath.DecreaseLifeLimitSeconds();
                            if (newValue == 0) {
                                RemoveMessagePath(messagePath);
                            }
                        }
                    }
                }
                foreach (var messagePath in messagePaths) {
                    // isMatch表示该处路径是否可以通过该消息，因为有些路径的PathId属性不为Guid.Empty，非空PathId的路径只允许特定标识造型的消息通过
                    // PathId可以认为是路径的形状，唯一的PathId表明该路径具有唯一的形状从而只允许和路径的形状一样的消息结构体穿过
                    bool isMatch = messagePath.PathId == Guid.Empty;
                    if (!isMatch) {
                        if (message is IEvent evt) {
                            isMatch = messagePath.PathId == evt.BornPathId;
                        }
                        else if (message is ICmd cmd) {
                            isMatch = messagePath.PathId == cmd.Id;
                        }
                    }
                    if (isMatch && messagePath.ViaTimesLimit > 0) {
                        // ViaTimesLimite小于0表示是不限定通过的次数的路径，不限定通过的次数的路径不需要消息每通过一次递减一次ViaTimesLimit计数
                        messagePath.DecreaseViaTimesLimit(onDownToZero: RemoveMessagePath);
                    }
                    if (!messagePath.IsEnabled) {
                        continue;
                    }
                    if (isMatch) {
                        switch (messagePath.LogType) {
                            case LogEnum.DevConsole:
                                if (DevMode.IsDevMode) {
                                    Write.DevDebug($"({typeof(TMessage).Name})->({messagePath.Location.Name}){messagePath.Description}");
                                }
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

        public void AddMessagePath<TMessage>(MessagePath<TMessage> path) {
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }
            MessagePathSet<TMessage>.Instance.AddMessagePath(path);
            MessagePathAdded?.Invoke(path);
        }

        public void RemoveMessagePath(IMessagePathId pathId) {
            if (pathId == null) {
                return;
            }
            MessagePathSetSet.RemoveMessagePath(pathId);
            MessagePathRemoved?.Invoke(pathId);
        }
        #endregion
    }
}
