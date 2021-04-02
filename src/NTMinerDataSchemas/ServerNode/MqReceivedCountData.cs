namespace NTMiner.ServerNode {
    public class MqReceivedCountData : IMqRoutingCountData {
        public MqReceivedCountData() { }

        public string RoutingKey { get; set; }

        public string Queue { get; set; }

        public long Count { get; set; }
    }
}
