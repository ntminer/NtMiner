namespace NTMiner.Bus {
    using System;
    using System.Collections.Generic;

    public class MessageDispatcher : IMessageDispatcher {
        private readonly Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();
        private readonly Dictionary<string, List<IHandlerId>> _paths = new Dictionary<string, List<IHandlerId>>();
        private readonly object _locker = new object();

        public event Action<IHandlerId> Connected;
        public event Action<IHandlerId> Disconnected;

        #region IMessageDispatcher Members
        public void Dispatch<TMessage>(TMessage message) {
            if (message == null) {
                throw new ArgumentNullException(nameof(message));
            }
            var messageType = typeof(TMessage);
            MessageTypeAttribute messageTypeDescription = MessageTypeAttribute.GetMessageTypeDescription(messageType);
            if (_handlers.ContainsKey(messageType)) {
                var messageHandlers = _handlers[messageType].ToArray();
                foreach (var messageHandler in messageHandlers) {
                    var tMessageHandler = (DelegateHandler<TMessage>)messageHandler;
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
                    tMessageHandler.Handle(message);
                }
            }
            else if (!messageTypeDescription.IsCanNoHandler) {
                Write.DevWarn(messageType.FullName + "类型的消息没有对应的处理器");
            }
        }

        public void Connect<TMessage>(DelegateHandler<TMessage> handler) {
            if (handler == null) {
                throw new ArgumentNullException(nameof(handler));
            }
            lock (_locker) {
                var keyType = typeof(TMessage);

                var handlerId = handler;
                if (!_paths.ContainsKey(handlerId.HandlerPath)) {
                    _paths.Add(handlerId.HandlerPath, new List<IHandlerId> { handlerId });
                }
                else {
                    List<IHandlerId> handlerIds = _paths[handlerId.HandlerPath];
                    if (handlerIds.Count == 1) {
                        Write.DevWarn($"重复的路径:{handlerIds[0].HandlerPath} {handlerIds[0].Description}");
                    }
                    handlerIds.Add(handlerId);
                    Write.DevWarn($"重复的路径:{handlerId.HandlerPath} {handlerId.Description}");
                }
                if (_handlers.ContainsKey(keyType)) {
                    var registeredHandlers = _handlers[keyType];
                    if (registeredHandlers.Count > 0 && typeof(ICmd).IsAssignableFrom(keyType)) {
                        // 因为一种命令只应被一个处理器处理，命令实际上可以设计为不走总线，
                        // 之所以设计为统一走总线只是为了将通过命令类型集中表达起文档作用。
                        throw new Exception($"一种命令只应被一个处理器处理:{typeof(TMessage).Name}");
                    }
                    if (!registeredHandlers.Contains(handler)) {
                        registeredHandlers.Add(handler);
                    }
                }
                else {
                    var registeredHandlers = new List<dynamic> { handler };
                    _handlers.Add(keyType, registeredHandlers);
                }
                Connected?.Invoke(handlerId);
            }
        }

        public void Disconnect(IHandlerId handlerId) {
            if (handlerId == null) {
                return;
            }
            lock (_locker) {
                _paths.Remove(handlerId.HandlerPath);
                var keyType = handlerId.MessageType;
                if (_handlers.ContainsKey(keyType) &&
                    _handlers[keyType] != null &&
                    _handlers[keyType].Count > 0 &&
                    _handlers[keyType].Contains(handlerId)) {
                    _handlers[keyType].Remove(handlerId);
                    Write.DevDebug("拆除路径" + handlerId.HandlerPath);
                    Disconnected?.Invoke(handlerId);
                }
            }
        }
        #endregion
    }
}
