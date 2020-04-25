using System;

namespace NTMiner.Core.MinerServer {
    [DataSchemaId("1FF2A8DE-3E76-48E8-992D-CA714789FDE0")]
    public class ServerMessageData : IServerMessage {
        private string _content;
        private DateTime _timestamp;

        public ServerMessageData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Provider { get; set; }

        public string MessageType { get; set; }

        public string Content {
            get => _content ?? string.Empty;
            set {
                _content = value ?? string.Empty;
            }
        }

        public DateTime Timestamp {
            get => _timestamp;
            set {
                value = value == DateTime.MinValue ? DateTime.Now : value;
                _timestamp = value;
            }
        }

        public bool IsDeleted { get; set; }
    }
}
