using System;

namespace NTMiner.MinerServer {
    public class ServerMessageData : IServerMessage {
        public ServerMessageData() { }

        public ServerMessageData(IServerMessage data) {
            Id = data.Id == Guid.Empty ? Guid.NewGuid() : data.Id;
            Provider = data.Provider;
            MessageType = data.MessageType;
            Content = data.Content;
            Timestamp = data.Timestamp == DateTime.MinValue ? DateTime.Now : data.Timestamp;
            IsDeleted = false;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Provider { get; set; }

        public string MessageType { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }

        public bool IsDeleted { get; set; }
    }
}
