using System.Collections.Generic;

namespace NTMiner.Bus {
    public static class MessageHandlerExtensions {

        public static IPathId AddToCollection(this IPathId handler, List<IPathId> handlers) {
            if (!handlers.Contains(handler)) {
                handlers.Add(handler);
            }
            return handler;
        }
    }
}
