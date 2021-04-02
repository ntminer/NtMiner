using System.Collections.Generic;

namespace NTMiner.ServerNode {
    public class MqCountData {
        public MqCountData() {
            this.ReceivedCounts = new List<MqReceivedCountData>();
            this.SendCounts = new List<MqSendCountData>();
        }

        public string AppId { get; set; }
        public List<MqReceivedCountData> ReceivedCounts { get; set; }
        public List<MqSendCountData> SendCounts { get; set; }
    }
}
