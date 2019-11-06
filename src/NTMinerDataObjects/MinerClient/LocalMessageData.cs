using System;

namespace NTMiner.MinerClient {
    public class LocalMessageData : ILocalMessage, IDbEntity<Guid> {
        public LocalMessageData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Channel { get; set; }

        public string Provider { get; set; }

        public string MessageType { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
