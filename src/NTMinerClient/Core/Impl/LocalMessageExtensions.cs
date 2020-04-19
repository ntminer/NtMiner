namespace NTMiner.Core.Impl {
    public static class LocalMessageExtensions {
        public static LocalMessageDto ToDto(this ILocalMessage localMessage) {
            LocalMessageChannel channel = LocalMessageChannel.Unspecified;
            LocalMessageType messageType = LocalMessageType.Info;
            if (localMessage.Channel.TryParse(out LocalMessageChannel localMessageChannel)) {
                channel = localMessageChannel;
            }
            if (localMessage.MessageType.TryParse(out LocalMessageType localMessageType)) {
                messageType = localMessageType;
            }
            return new LocalMessageDto {
                Channel = channel,
                Content = localMessage.Content,
                MessageType = messageType,
                Provider = localMessage.Provider,
                Timestamp = Timestamp.GetTimestamp(localMessage.Timestamp)
            };
        }
    }
}
