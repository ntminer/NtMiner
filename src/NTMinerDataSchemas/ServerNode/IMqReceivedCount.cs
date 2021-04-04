namespace NTMiner.ServerNode {
    public interface IMqReceivedCount {
        string Queue { get; }
        string RoutingKey { get; }
        long Count { get; }
    }
}
