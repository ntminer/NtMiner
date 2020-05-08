namespace NTMiner {
    public static class PagingRequestExtensions {
        /// <summary>
        /// 如果PageIndex小于1视为1，如果PageSize大于100视为100。
        /// </summary>
        /// <param name="request"></param>
        public static void PagingTrim(this IPagingRequest request) {
            if (request == null) {
                return;
            }
            if (request.PageIndex < 1) {
                request.PageIndex = 1;
            }
            if (request.PageSize > 100) {
                request.PageSize = 100;
            }
        }
    }
}
