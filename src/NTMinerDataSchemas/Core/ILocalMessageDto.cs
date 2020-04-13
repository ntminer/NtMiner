namespace NTMiner.Core {
    public interface ILocalMessageDto {
        long Timestamp { get; }
        LocalMessageChannel Channel { get; }
        LocalMessageType MessageType { get; }
        string Provider { get; }
        string Content { get; }
    }
}
