using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;
using System.Net.Http;

namespace NTMiner.Services.Official {
    public partial class ClientDataBinaryService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IClientDataBinaryController<HttpResponseMessage>>();

        internal ClientDataBinaryService() {
        }

        #region QueryClientsAsync
        public void QueryClientsAsync(QueryClientsRequest query, Action<QueryClientsResponse, Exception> callback) {
            RpcRoot.JsonRequestBinaryResponseRpcHelper.SignPostAsync(
                RpcRoot.OfficialServerHost, 
                RpcRoot.OfficialServerPort, 
                _controllerName, 
                nameof(IClientDataBinaryController<HttpResponseMessage>.QueryClients), 
                data: query, 
                callback);
        }
        #endregion
    }
}
