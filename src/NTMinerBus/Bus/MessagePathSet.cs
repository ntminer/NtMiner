namespace NTMiner.Bus {
    using System;
    using System.Collections.Generic;

    public class MessagePathSet : IMessagePathSet {
        private readonly Dictionary<Type, List<object>> _pathDicByMessageType = new Dictionary<Type, List<object>>();
        private readonly Dictionary<string, List<IMessagePathId>> _paths = new Dictionary<string, List<IMessagePathId>>();
        private readonly object _locker = new object();

        public event Action<IMessagePathId> Added;
        public event Action<IMessagePathId> Removed;

        #region IMessageDispatcher Members
        public IEnumerable<IMessagePathId> GetAllPaths() {
            lock (_locker) {
                foreach (var item in _paths) {
                    foreach (var path in item.Value) {
                        yield return path;
                    }
                }
            }
        }

        public void Route<TMessage>(TMessage message) where TMessage : IMessage {
            if (message == null) {
                throw new ArgumentNullException(nameof(message));
            }
            var messageType = typeof(TMessage);
            if (_pathDicByMessageType.TryGetValue(messageType, out List<object> list)) {
                var messagePaths = list.ToArray();
                foreach (var messagePath in messagePaths) {
                    var tMessagePath = (MessagePath<TMessage>)messagePath;
                    // isMatch表示该处路径是否可以通过该消息，因为有些路径的PathId属性不为Guid.Empty，非空PathId的路径只允许特定标识造型的消息通过
                    // PathId可以认为是路径的形状，唯一的PathId表明该路径具有唯一的形状从而只允许和路径的形状一样的消息结构体穿过
                    bool isMatch = tMessagePath.PathId == Guid.Empty || message is ICmd;
                    if (!isMatch && message is IEvent evt) {
                        isMatch = tMessagePath.PathId == evt.BornPathId;
                    }
                    if (isMatch) {
                        // ViaLimite小于0表示是不限定通过的次数的路径，不限定通过的次数的路径不需要消息每通过一次递减一次ViaLimit计数
                        if (tMessagePath.ViaLimit > 0) {
                            lock (tMessagePath.Locker) {
                                if (tMessagePath.ViaLimit > 0) {
                                    tMessagePath.ViaLimit--;
                                    if (tMessagePath.ViaLimit == 0) {
                                        // ViaLimit递减到0从路径列表中移除该路径
                                        Remove(tMessagePath);
                                    }
                                }
                            }
                        }
                    }
                    if (!tMessagePath.IsEnabled) {
                        continue;
                    }
                    if (isMatch) {
                        switch (tMessagePath.LogType) {
                            case LogEnum.DevConsole:
                                if (DevMode.IsDevMode) {
                                    Write.DevDebug($"({messageType.Name})->({tMessagePath.Location.Name}){tMessagePath.Description}");
                                }
                                break;
                            case LogEnum.Log:
                                Logger.InfoDebugLine($"({messageType.Name})->({tMessagePath.Location.Name}){tMessagePath.Description}");
                                break;
                            case LogEnum.None:
                            default:
                                break;
                        }
                        tMessagePath.Go(message);
                    }
                }
            }
            else {
                MessageTypeAttribute messageTypeAttr = MessageTypeAttribute.GetMessageTypeAttribute(messageType);
                if (!messageTypeAttr.IsCanNoHandler) {
                    Write.DevWarn(messageType.FullName + "类型的消息没有对应的处理器");
                }
            }
        }

        public void Add<TMessage>(MessagePath<TMessage> path) {
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }
            lock (_locker) {
                var messageType = typeof(TMessage);

                var pathId = path;
                if (!_paths.ContainsKey(pathId.Path)) {
                    _paths.Add(pathId.Path, new List<IMessagePathId> { pathId });
                }
                else {
                    List<IMessagePathId> handlerIds = _paths[pathId.Path];
                    if (handlerIds.Count == 1) {
                        Write.DevWarn($"重复的路径:{handlerIds[0].Path} {handlerIds[0].Description}");
                    }
                    handlerIds.Add(pathId);
                    Write.DevWarn($"重复的路径:{pathId.Path} {pathId.Description}");
                }
                if (_pathDicByMessageType.ContainsKey(messageType)) {
                    var registeredHandlers = _pathDicByMessageType[messageType];
                    if (registeredHandlers.Count > 0 && typeof(ICmd).IsAssignableFrom(messageType)) {
                        // 因为一种命令只应被一个处理器处理，命令实际上可以设计为不走总线，
                        // 之所以设计为统一走总线只是为了通过将命令类型集中表达以起文档作用。
                        throw new Exception($"一种命令只应被一个处理器处理:{typeof(TMessage).Name}");
                    }
                    if (!registeredHandlers.Contains(path)) {
                        registeredHandlers.Add(path);
                    }
                }
                else {
                    var registeredHandlers = new List<dynamic> { path };
                    _pathDicByMessageType.Add(messageType, registeredHandlers);
                }
                Added?.Invoke(pathId);
            }
        }

        public void Remove(IMessagePathId handlerId) {
            if (handlerId == null) {
                return;
            }
            lock (_locker) {
                _paths.Remove(handlerId.Path);
                var messageType = handlerId.MessageType;
                if (_pathDicByMessageType.ContainsKey(messageType) &&
                    _pathDicByMessageType[messageType] != null &&
                    _pathDicByMessageType[messageType].Count > 0 &&
                    _pathDicByMessageType[messageType].Contains(handlerId)) {
                    _pathDicByMessageType[messageType].Remove(handlerId);
                    Write.DevDebug("拆除路径" + handlerId.Path);
                    Removed?.Invoke(handlerId);
                }
            }
        }
        #endregion
    }
}
