using System;

namespace NTMiner {
    public class WsMessage {
        public WsMessage() { }

        public WsMessage(Guid messageId) {
            if (messageId != Guid.Empty) {
                this.messageId = messageId.ToString();
            }
        }

        public string messageId { get; set; }
        public string action { get; set; }

        public int code { get; set; }
        public string phrase { get; set; }
        public string des { get; set; }

        public object data { get; set; }
    }
}
