namespace NTMiner.ServerNode {
    public interface IMqSendCount {
        string RoutingKey { get; }
        long Count { get; }
    }
}
