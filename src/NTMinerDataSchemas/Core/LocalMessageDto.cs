namespace NTMiner.Core {
    public class LocalMessageDto : ILocalMessageDto {
        public LocalMessageDto() { }

        public long Timestamp { get; set; }

        public LocalMessageChannel Channel { get; set; }

        public LocalMessageType MessageType { get; set; }

        public string Provider { get; set; }

        public string Content { get; set; }
    }
}
