using NTMiner.ServerNode;

namespace NTMiner.Controllers {
    public interface IActionCountController {
        QueryActionCountsResponse QueryActionCounts(QueryActionCountsRequest request);
    }
}
