namespace NTMiner.ServerNode {
    public interface IVarWsServerNode : IVarServerState {
        int MinerClientWsSessionCount { get; }
        int MinerStudioWsSessionCount { get; }
        int MinerClientSessionCount { get; }
        int MinerStudioSessionCount { get; }
    }
}
