using System;

namespace NTMiner.Bus {
    public static class MessageDispatcherExtensions {
        public static DelegatePath<TMessage> Connect<TMessage>(this IMessageDispatcher dispatcher, Type location, string description, LogEnum logType, Action<TMessage> action) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }
            DelegatePath<TMessage> handler = new DelegatePath<TMessage>(location, description, logType, action);
            dispatcher.Connect(handler);
            return handler;
        }
    }
}
