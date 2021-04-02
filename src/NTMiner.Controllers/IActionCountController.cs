using NTMiner.ServerNode;

namespace NTMiner.Controllers {
    public interface IActionCountController {
        /// <summary>
        /// 需签名
        /// </summary>
        QueryActionCountsResponse QueryActionCounts(QueryActionCountsRequest request);
    }
}
