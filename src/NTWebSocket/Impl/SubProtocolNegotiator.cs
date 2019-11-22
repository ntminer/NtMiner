using System.Collections.Generic;
using System.Linq;

namespace NTWebSocket.Impl {
    public static class SubProtocolNegotiator {
        public static string Negotiate(IEnumerable<string> server, IEnumerable<string> client) {
            if (!server.Any() || !client.Any()) {
                return null;
            }

            var matches = client.Intersect(server);
            if (!matches.Any()) {
                throw new SubProtocolNegotiationFailureException("Unable to negotiate a subprotocol");
            }
            return matches.First();
        }
    }
}
