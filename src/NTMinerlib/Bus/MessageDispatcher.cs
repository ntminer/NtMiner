
namespace NTMiner.Bus {
    using System;
    using System.Collections.Generic;

    public class MessageDispatcher : IMessageDispatcher {
        private readonly Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();

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
                    if (tMessageHandler.HandlerId.LogType == LogEnum.Log) {
                        Logger.InfoDebugLine($"({messageType.Name}) -> ({tMessageHandler.HandlerId.Location.Name}){tMessageHandler.HandlerId.Description}");
                    }
                    if (tMessageHandler.HandlerId.LogType == LogEnum.Console) {
                        Write.DevLine($"({messageType.Name}) -> ({tMessageHandler.HandlerId.Location.Name}){tMessageHandler.HandlerId.Description}");
                    }
                    this.Dispatching?.Invoke(this, evtArgs);
                    try {
                        tMessageHandler.Handle(message);
                        this.Dispatched?.Invoke(this, evtArgs);
                    }
                    catch (Exception e) {
                        this.DispatchFailed?.Invoke(this, evtArgs);
                        Logger.ErrorDebugLine(tMessageHandler.GetType().FullName + ":" + messageType.FullName + ":" + e.Message, e);
                        throw;
                    }
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
            var keyType = typeof(TMessage);

            if (_handlers.ContainsKey(keyType)) {
                if (typeof(ICmd).IsAssignableFrom(keyType)) {
                    throw new Exception($"one {typeof(TMessage).Name} cmd can be handle and only be handle by one handler");
                }
                var registeredHandlers = _handlers[keyType];
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

        public void UnRegister<TMessage>(DelegateHandler<TMessage> handler) {
            if (handler == null) {
                return;
            }
            var keyType = typeof(TMessage);
            if (_handlers.ContainsKey(keyType) &&
                _handlers[keyType] != null &&
                _handlers[keyType].Count > 0 &&
                _handlers[keyType].Contains(handler)) {
                _handlers[keyType].Remove(handler);
            }
        }

        public event EventHandler<MessageDispatchEventArgs> Dispatching;

        public event EventHandler<MessageDispatchEventArgs> DispatchFailed;

        public event EventHandler<MessageDispatchEventArgs> Dispatched;
        #endregion
    }
}
