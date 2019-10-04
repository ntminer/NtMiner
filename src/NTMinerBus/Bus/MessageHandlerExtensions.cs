using System.Collections.Generic;

namespace NTMiner.Bus {
    public static class MessageHandlerExtensions {

        public static IHandlerId AddToCollection(this IHandlerId handler, List<IHandlerId> handlers) {
            if (!handlers.Contains(handler)) {
                handlers.Add(handler);
            }
            return handler;
        }
    }
}
