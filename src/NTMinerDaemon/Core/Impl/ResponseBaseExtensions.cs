namespace NTMiner.Core.Impl {
    public static class ResponseBaseExtensions {
        public static OperationResultData ToOperationResult(this ResponseBase response) {
            if (response == null) {
                return null;
            }
            return new OperationResultData {
                Timestamp = Timestamp.GetTimestamp(),
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description
            };
        }
    }
}
