
namespace NTMiner.Bus {
    using System;
    using System.Collections.Generic;

    public class MessageDispatcher : IMessageDispatcher {
        private readonly Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();
        private readonly HashSet<string> _paths = new HashSet<string>();
        private readonly object _locker = new object();

        #region IMessageDispatcher Members
        public void DispatchMessage<TMessage>(TMessage message) {
            if (message == null) {
                throw new ArgumentNullException("message");
            }
            var messageType = typeof(TMessage);
            MessageTypeAttribute messageTypeDescription = MessageTypeAttribute.GetMessageTypeDescription(messageType);
            if (_handlers.ContainsKey(messageType)) {
                var messageHandlers = _handlers[messageType].ToArray();
                foreach (var messageHandler in messageHandlers) {
                    var tMessageHandler = (DelegateHandler<TMessage>)messageHandler;
                    var evtArgs = new MessageDispatchEventArgs(message, messageHandler.GetType(), messageHandler);
                    switch (tMessageHandler.HandlerId.LogType) {
                        case LogEnum.DevConsole:
                            if (DevMode.IsDevMode) {
                                Write.DevLine($"({messageType.Name})->({tMessageHandler.HandlerId.Location.Name}){tMessageHandler.HandlerId.Description}");
                            }
                            break;
                        case LogEnum.UserConsole:
                            Write.UserLine($"({messageType.Name})->({tMessageHandler.HandlerId.Location.Name}){tMessageHandler.HandlerId.Description}", ConsoleColor.Gray);
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
                Write.DevLine(messageType.FullName + "类型的消息没有对应的处理器");
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
                    Write.DevLine($"重复的路径:{handlerId.HandlerPath}", ConsoleColor.Red);
                }
                if (_handlers.ContainsKey(keyType)) {
                    var registeredHandlers = _handlers[keyType];
                    if (registeredHandlers.Count > 0 && typeof(ICmd).IsAssignableFrom(keyType)) {
                        throw new Exception($"one {typeof(TMessage).Name} cmd can be handle and only be handle by one handler");
                    }
                    if (registeredHandlers != null) {
                        if (!registeredHandlers.Contains(handler))
                            registeredHandlers.Add(handler);
                    }
                    else {
                        registeredHandlers = new List<dynamic> { handler };
                        _handlers.Add(keyType, registeredHandlers);
                    }
                }
                else {
                    var registeredHandlers = new List<dynamic> { handler };
                    _handlers.Add(keyType, registeredHandlers);
                }
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
                }
            }
        }
        #endregion
    }
}
