using NTMiner.User;
using WebSocketSharp.Server;

namespace NTMiner {
    public static class WebSocketBehaviorExtensions {
        public static bool TryGetUser(this WebSocketBehavior behavior, out WsUserName wsUserName, out UserData userData) {
            string base64String = behavior.Context.User.Identity.Name;
            return WsRoot.TryGetUser(base64String, out wsUserName, out userData);
        }
    }
}
