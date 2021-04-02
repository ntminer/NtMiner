namespace NTMiner.ServerNode {
    public class MqSendCountData {
        public MqSendCountData() { }

        public string RoutingKey { get; set; }
        public long Count { get; set; }
    }
}
