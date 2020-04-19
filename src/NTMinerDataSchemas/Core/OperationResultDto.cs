namespace NTMiner.Core {
    public class OperationResultDto : IOperationResult {
        public OperationResultDto() { }

        public long Timestamp { get; set; }

        public int StateCode { get; set; }

        public string ReasonPhrase { get; set; }

        public string Description { get; set; }
    }
}
