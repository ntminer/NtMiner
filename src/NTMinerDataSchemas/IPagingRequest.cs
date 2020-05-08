namespace NTMiner {
    public interface IPagingRequest : IRequest {
        /// <summary>
        /// 注意它是1基的
        /// </summary>
        int PageIndex { get; set; }
        int PageSize { get; set; }
    }
}
