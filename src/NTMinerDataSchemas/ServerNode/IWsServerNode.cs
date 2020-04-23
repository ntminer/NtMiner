namespace NTMiner.ServerNode {
    public interface IWsServerNode : IServerState {
        int MinerClientWsSessionCount { get; }
        int MinerStudioWsSessionCount { get; }
        int MinerClientSessionCount { get; }
        int MinerStudioSessionCount { get; }
    }
}
