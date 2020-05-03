using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;
using System.Net.Http;

namespace NTMiner.Services.Official {
    public class ClientDataBinaryService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IClientDataBinaryController<HttpResponseMessage>>();

        public ClientDataBinaryService() {
        }

        #region QueryClientsAsync
        public void QueryClientsAsync(QueryClientsRequest query, Action<QueryClientsResponse, Exception> callback) {
            BinaryResponseRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IClientDataBinaryController<HttpResponseMessage>.QueryClients), data: query, callback);
        }
        #endregion
    }
}
