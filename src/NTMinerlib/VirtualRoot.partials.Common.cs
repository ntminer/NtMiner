using NTMiner.User;
using System;

namespace NTMiner {
    public static partial class VirtualRoot {
        public static readonly bool IsGEWin10 = Environment.OSVersion.Version >= new Version(6, 2);
        public static readonly bool IsLTWin10 = Environment.OSVersion.Version < new Version(6, 2);
        public static RpcUser RpcUser { get; private set; } = RpcUser.Empty;

        public static void SetRpcUser(RpcUser rpcUser) {
            RpcUser = rpcUser;
        }
    }
}
