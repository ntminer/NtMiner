namespace NTMiner.ServerNode {
    public class MqReceivedCountData : IMqReceivedCount {
        public MqReceivedCountData() { }

        public string RoutingKey { get; set; }

        public string Queue { get; set; }

        public long Count { get; set; }
    }
}
