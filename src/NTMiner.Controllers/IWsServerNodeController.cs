using NTMiner.ServerNode;

namespace NTMiner.Controllers {
    public interface IWsServerNodeController {
        DataResponse<string> GetNodeAddress(GetWsServerNodeAddressRequest request);
    }
}
