using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NTMiner.Services.Official {
    public class ServerMessageBinaryService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IServerMessageBinaryController<HttpResponseMessage>>();

        internal ServerMessageBinaryService() {
        }

        #region GetServerMessagesAsync
        public void GetServerMessagesAsync(DateTime timestamp, Action<DataResponse<List<ServerMessageData>>, Exception> callback) {
            ServerMessagesRequest request = new ServerMessagesRequest {
                Timestamp = Timestamp.GetTimestamp(timestamp)
            };
            RpcRoot.JsonRequestBinaryResponseRpcHelper.PostAsync(
                _controllerName, 
                nameof(IServerMessageBinaryController<HttpResponseMessage>.ServerMessages), 
                request, 
                callback);
        }
        #endregion
    }
}
