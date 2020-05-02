using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Official {
    public partial class ClientDataService {
        private readonly string _controllerName = JsonRpcRoot.GetControllerName<IClientDataController>();

        public ClientDataService() {
        }

        #region QueryClientsAsync
        public void QueryClientsAsync(QueryClientsRequest query, Action<QueryClientsResponse, Exception> callback) {
            JsonRpcRoot.SignPostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IClientDataController.QueryClients), data: query, callback);
        }
        #endregion

        #region UpdateClientAsync
        public void UpdateClientAsync(string objectId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
            UpdateClientRequest request = new UpdateClientRequest {
                ObjectId = objectId,
                PropertyName = propertyName,
                Value = value
            };
            JsonRpcRoot.SignPostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IClientDataController.UpdateClient), data: request, callback);
        }
        #endregion

        #region UpdateClientsAsync
        public void UpdateClientsAsync(string propertyName, Dictionary<string, object> values, Action<ResponseBase, Exception> callback) {
            UpdateClientsRequest request = new UpdateClientsRequest {
                PropertyName = propertyName,
                Values = values
            };
            JsonRpcRoot.SignPostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IClientDataController.UpdateClients), data: request, callback);
        }
        #endregion

        public void RemoveClientsAsync(List<string> objectIds, Action<ResponseBase, Exception> callback) {
            var request = new MinerIdsRequest {
                ObjectIds = objectIds
            };
            JsonRpcRoot.SignPostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IClientDataController.RemoveClients), data: request, callback);
        }
    }
}
