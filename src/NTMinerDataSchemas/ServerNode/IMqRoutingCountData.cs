namespace NTMiner.ServerNode {
    public interface IMqRoutingCountData {
        string RoutingKey { get; }
        string Queue { get; }
        long Count { get; }
    }
}
