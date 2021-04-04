namespace NTMiner.ServerNode {
    public interface IMqReceivedCount {
        string RoutingKey { get; }
        string Queue { get; }
        long Count { get; }
    }
}
