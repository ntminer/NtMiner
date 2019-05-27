
namespace NTMiner.Bus {
    using System;
    using System.Collections.Generic;

    public class MessageDispatcher : IMessageDispatcher {
        private readonly Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();
        private readonly HashSet<string> _paths = new HashSet<string>();
        private readonly List<IHandlerId> _handlerIds = new List<IHandlerId>();
        private readonly object _locker = new object();

        public IEnumerable<IHandlerId> HandlerIds {
            get {
                return _handlerIds;
            }
        }

        public event Action<IHandlerId> HandlerIdAdded;
        public event Action<IHandlerId> HandlerIdRemoved;

        #region IMessageDispatcher Members
        public void DispatchMessage<TMessage>(TMessage message) {
            if (message == null) {
                throw new ArgumentNullException(nameof(message));
            }
            var messageType = typeof(TMessage);
            MessageTypeAttribute messageTypeDescription = MessageTypeAttribute.GetMessageTypeDescription(messageType);
            if (_handlers.ContainsKey(messageType)) {
                var messageHandlers = _handlers[messageType].ToArray();
                foreach (var messageHandler in messageHandlers) {
                    var tMessageHandler = (DelegateHandler<TMessage>)messageHandler;
                    if (!tMessageHandler.HandlerId.IsEnabled) {
                        continue;
                    }
                    var evtArgs = new MessageDispatchEventArgs(message, messageHandler.GetType(), messageHandler);
                    switch (tMessageHandler.HandlerId.LogType) {
                        case LogEnum.DevConsole:
                            if (DevMode.IsDevMode) {
                                Write.DevDebug($"({messageType.Name})->({tMessageHandler.HandlerId.Location.Name}){tMessageHandler.HandlerId.Description}");
                            }
                            break;
                        case LogEnum.UserConsole:
                            Write.UserInfo($"({messageType.Name})->({tMessageHandler.HandlerId.Location.Name}){tMessageHandler.HandlerId.Description}");
                            break;
                        case LogEnum.Log:
                            Logger.InfoDebugLine($"({messageType.Name})->({tMessageHandler.HandlerId.Location.Name}){tMessageHandler.HandlerId.Description}");
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

        public void Register<TMessage>(DelegateHandler<TMessage> handler) {
            if (handler == null) {
                throw new ArgumentNullException(nameof(handler));
            }
            lock (_locker) {
                var keyType = typeof(TMessage);

                var handlerId = handler.HandlerId;
                if (!_paths.Contains(handlerId.HandlerPath)) {
                    _paths.Add(handlerId.HandlerPath);
                }
                else {
                    Write.DevWarn($"重复的路径:{handlerId.HandlerPath}");
                }
                if (_handlers.ContainsKey(keyType)) {
                    var registeredHandlers = _handlers[keyType];
                    if (registeredHandlers.Count > 0 && typeof(ICmd).IsAssignableFrom(keyType)) {
                        throw new Exception($"one {typeof(TMessage).Name} cmd can be handle and only be handle by one handler");
                    }
                    if (!registeredHandlers.Contains(handler)) {
                        registeredHandlers.Add(handler);
                    }
                }
                else {
                    var registeredHandlers = new List<dynamic> { handler };
                    _handlers.Add(keyType, registeredHandlers);
                }
                _handlerIds.Add(handler.HandlerId);
                HandlerIdAdded?.Invoke(handler.HandlerId);
            }
        }

        public void UnRegister(IDelegateHandler handler) {
            if (handler == null) {
                return;
            }
            lock (_locker) {
                var handlerId = handler.HandlerId;
                _paths.Remove(handlerId.HandlerPath);
                var keyType = handlerId.MessageType;
                if (_handlers.ContainsKey(keyType) &&
                    _handlers[keyType] != null &&
                    _handlers[keyType].Count > 0 &&
                    _handlers[keyType].Contains(handler)) {
                    _handlers[keyType].Remove(handler);
                    _handlerIds.Remove(handlerId);
                    HandlerIdRemoved?.Invoke(handlerId);
                    Write.DevDebug("拆除路径" + handler.HandlerId.HandlerPath);
                }
            }
        }
        #endregion
    }
}
