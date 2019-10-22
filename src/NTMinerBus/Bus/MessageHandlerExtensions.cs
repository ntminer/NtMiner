using System.Collections.Generic;

namespace NTMiner.Bus {
    public static class MessageHandlerExtensions {

        public static IMessagePathId AddToCollection(this IMessagePathId handler, List<IMessagePathId> handlers) {
            if (!handlers.Contains(handler)) {
                handlers.Add(handler);
            }
            return handler;
        }
    }
}
