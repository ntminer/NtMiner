namespace NTMiner.Bus {
    using System;
    using System.Collections.Generic;

    public class MessageDispatcher : IMessageDispatcher {
        private readonly Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();
        private readonly Dictionary<string, List<IMessagePathId>> _paths = new Dictionary<string, List<IMessagePathId>>();
        private readonly object _locker = new object();

        public event Action<IMessagePathId> Connected;
        public event Action<IMessagePathId> Disconnected;

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

        public void Dispatch<TMessage>(TMessage message) {
            if (message == null) {
                throw new ArgumentNullException(nameof(message));
            }
            var messageType = typeof(TMessage);
            if (_handlers.ContainsKey(messageType)) {
                var messageHandlers = _handlers[messageType].ToArray();
                foreach (var messageHandler in messageHandlers) {
                    var tMessageHandler = (MessagePath<TMessage>)messageHandler;
                    if (tMessageHandler.ViaLimit > 0) {
                        lock (tMessageHandler) {
                            if (tMessageHandler.ViaLimit > 0) {
                                tMessageHandler.ViaLimit--;
                                if (tMessageHandler.ViaLimit == 0) {
                                    Disconnect(tMessageHandler);
                                }
                            }
                        }
                    }
                    if (!tMessageHandler.IsEnabled) {
                        continue;
                    }
                    switch (tMessageHandler.LogType) {
                        case LogEnum.DevConsole:
                            if (DevMode.IsDevMode) {
                                Write.DevDebug($"({messageType.Name})->({tMessageHandler.Location.Name}){tMessageHandler.Description}");
                            }
                            break;
                        case LogEnum.Log:
                            Logger.InfoDebugLine($"({messageType.Name})->({tMessageHandler.Location.Name}){tMessageHandler.Description}");
                            break;
                        case LogEnum.None:
                        default:
                            break;
                    }
                    tMessageHandler.Run(message);
                }
            }
            else {
                MessageTypeAttribute messageTypeAttr = MessageTypeAttribute.GetMessageTypeAttribute(messageType);
                if (!messageTypeAttr.IsCanNoHandler) {
                    Write.DevWarn(messageType.FullName + "类型的消息没有对应的处理器");
                }
            }
        }

        public void Connect<TMessage>(MessagePath<TMessage> path) {
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }
            lock (_locker) {
                var keyType = typeof(TMessage);

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
                if (_handlers.ContainsKey(keyType)) {
                    var registeredHandlers = _handlers[keyType];
                    if (registeredHandlers.Count > 0 && typeof(ICmd).IsAssignableFrom(keyType)) {
                        // 因为一种命令只应被一个处理器处理，命令实际上可以设计为不走总线，
                        // 之所以设计为统一走总线只是为了将通过命令类型集中表达起文档作用。
                        throw new Exception($"一种命令只应被一个处理器处理:{typeof(TMessage).Name}");
                    }
                    if (!registeredHandlers.Contains(path)) {
                        registeredHandlers.Add(path);
                    }
                }
                else {
                    var registeredHandlers = new List<dynamic> { path };
                    _handlers.Add(keyType, registeredHandlers);
                }
                Connected?.Invoke(pathId);
            }
        }

        public void Disconnect(IMessagePathId handlerId) {
            if (handlerId == null) {
                return;
            }
            lock (_locker) {
                _paths.Remove(handlerId.Path);
                var keyType = handlerId.MessageType;
                if (_handlers.ContainsKey(keyType) &&
                    _handlers[keyType] != null &&
                    _handlers[keyType].Count > 0 &&
                    _handlers[keyType].Contains(handlerId)) {
                    _handlers[keyType].Remove(handlerId);
                    Write.DevDebug("拆除路径" + handlerId.Path);
                    Disconnected?.Invoke(handlerId);
                }
            }
        }
        #endregion
    }
}
