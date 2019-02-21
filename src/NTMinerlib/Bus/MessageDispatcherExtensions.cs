using System;
using System.Collections.Generic;

namespace NTMiner.Bus {
    public static class MessageDispatcherExtensions {
        private static readonly HashSet<string> _paths = new HashSet<string>();
        public static DelegateHandler<TMessage> Register<TMessage>(this IMessageDispatcher dispatcher, IHandlerId handlerId, Action<TMessage> action) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }
            if (!_paths.Contains(handlerId.HandlerPath)) {
                _paths.Add(handlerId.HandlerPath);
            }
            else {
                Write.DevLine($"重复的路径:{handlerId.HandlerPath}", ConsoleColor.Red);
            }
            DelegateHandler<TMessage> handler = new DelegateHandler<TMessage>(handlerId, action);
            dispatcher.Register(handler);
            return handler;
        }
    }
}
