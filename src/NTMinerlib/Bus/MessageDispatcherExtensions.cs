using System;

namespace NTMiner.Bus {
    public static class MessageDispatcherExtensions {
        public static DelegateHandler<TMessage> Register<TMessage>(this IMessageDispatcher dispatcher, IHandlerId handlerId, Action<TMessage> action) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }
            DelegateHandler<TMessage> handler = new DelegateHandler<TMessage>(handlerId, action);
            dispatcher.Register(handler);
            return handler;
        }
    }
}
