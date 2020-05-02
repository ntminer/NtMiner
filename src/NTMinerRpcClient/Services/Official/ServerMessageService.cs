using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Official {
    public class ServerMessageService {
        private readonly string _controllerName = JsonRpcRoot.GetControllerName<IServerMessageController>();

        public ServerMessageService() {
        }

        #region GetServerMessagesAsync
        public void GetServerMessagesAsync(DateTime timestamp, Action<DataResponse<List<ServerMessageData>>, Exception> callback) {
            ServerMessagesRequest request = new ServerMessagesRequest {
                Timestamp = Timestamp.GetTimestamp(timestamp)
            };
            JsonRpcRoot.PostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IServerMessageController.ServerMessages), request, callback);
        }
        #endregion

        #region AddOrUpdateServerMessageAsync
        public void AddOrUpdateServerMessageAsync(ServerMessageData entity, Action<ResponseBase, Exception> callback) {
            DataRequest<ServerMessageData> request = new DataRequest<ServerMessageData>() {
                Data = entity
            };
            JsonRpcRoot.SignPostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IServerMessageController.AddOrUpdateServerMessage), data: request, callback);
        }
        #endregion

        #region MarkDeleteServerMessageAsync
        public void MarkDeleteServerMessageAsync(Guid id, Action<ResponseBase, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid>() {
                Data = id
            };
            JsonRpcRoot.SignPostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IServerMessageController.MarkDeleteServerMessage), data: request, callback);
        }
        #endregion
    }
}
