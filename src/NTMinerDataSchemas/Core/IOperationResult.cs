namespace NTMiner.Core {
    public interface IOperationResult {
        long Timestamp { get; }
        int StateCode { get; }
        string ReasonPhrase { get; }
        string Description { get; }
    }
}
