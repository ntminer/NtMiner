using System;

namespace NTMiner.Core {
    public class LocalMessageData : ILocalMessage, IDbEntity<Guid> {
        public LocalMessageData() { }

        public static LocalMessageData Create(ILocalMessage data) {
            if (data is LocalMessageData r) {
                return r;
            }
            return new LocalMessageData {
                Id = data.Id,
                Channel = data.Channel,
                MessageType = data.MessageType,
                Provider = data.Provider,
                Content = data.Content,
                Timestamp = data.Timestamp
            };
        }

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
