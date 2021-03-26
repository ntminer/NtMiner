namespace NTMiner.ServerNode {
    public class MqRoutingCountData : IMqRoutingCountData {
        public MqRoutingCountData() { }

        public string RoutingKey { get; set; }

        public string Queue { get; set; }

        public long Count { get; set; }
    }
}
