using NTMiner.ServerNode;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IWsServerNodeController {
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<List<WsServerNodeState>> Nodes(object request);
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<string[]> NodeAddresses(object request);
        DataResponse<string> GetNodeAddress(GetWsServerNodeAddressRequest request);
    }
}
