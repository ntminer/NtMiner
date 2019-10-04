using System.Collections.Generic;

namespace NTMiner.Bus {
    public static class MessageHandlerExtensions {

        public static IMessageHandler AddToCollection(this IMessageHandler handler, List<IMessageHandler> handlers) {
            if (!handlers.Contains(handler)) {
                handlers.Add(handler);
            }
            return handler;
        }
    }
}
