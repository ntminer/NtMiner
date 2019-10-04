using System;

namespace NTMiner.Bus {
    public static class MessageDispatcherExtensions {
        public static DelegateHandler<TMessage> Connect<TMessage>(this IMessageDispatcher dispatcher, Type location, string description, LogEnum logType, Action<TMessage> action) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }
            DelegateHandler<TMessage> handler = new DelegateHandler<TMessage>(location, description, logType, action);
            dispatcher.Connect(handler);
            return handler;
        }
    }
}
