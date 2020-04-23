using NTMiner.ServerNode;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IWsServerNodeController {
        DataResponse<List<WsServerNodeState>> Nodes(SignRequest request);
        DataResponse<string[]> NodeAddresses(SignRequest request);
        DataResponse<string> GetNodeAddress(GetNodeAddressRequest request);

        ResponseBase ReportNodeState(WsServerNodeState state);
        ResponseBase RemoveNode(DataRequest<string> request);
    }
}
