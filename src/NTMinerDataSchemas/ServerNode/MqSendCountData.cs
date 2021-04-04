namespace NTMiner.ServerNode {
    public class MqSendCountData : IMqSendCount {
        public MqSendCountData() { }

        public string RoutingKey { get; set; }
        public long Count { get; set; }
    }
}
