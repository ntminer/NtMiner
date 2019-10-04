using System;

namespace NTMiner.Bus {
    public static class MessageDispatcherExtensions {
        public static DelegateHandler<TMessage> Register<TMessage>(this IMessageDispatcher dispatcher, Type messageType, Type location, string description, LogEnum logType, Action<TMessage> action) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }
            DelegateHandler<TMessage> handler = new DelegateHandler<TMessage>(messageType, location, description, logType, action);
            dispatcher.Register(handler);
            return handler;
        }
    }
}
