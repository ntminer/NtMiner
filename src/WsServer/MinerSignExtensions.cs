using NTMiner.Core.MinerServer;
using NTMiner.User;

namespace NTMiner {
    public static class MinerSignExtensions {
        public static void Update(this MinerSign minerSign, WsUserName wsUserName) {
            minerSign.OuterUserId = wsUserName.UserId;
        }
    }
}
