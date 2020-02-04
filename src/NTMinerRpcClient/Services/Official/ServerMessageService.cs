using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Official {
    public class ServerMessageService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IServerMessageController>();
        private readonly string _host;
        private readonly int _port;

        public ServerMessageService(string host, int port) {
            _host = host;
            _port = port;
        }

        #region GetServerMessagesAsync
        public void GetServerMessagesAsync(DateTime timestamp, Action<DataResponse<List<ServerMessageData>>, Exception> callback) {
            ServerMessagesRequest request = new ServerMessagesRequest {
                Timestamp = Timestamp.GetTimestamp(timestamp)
            };
            RpcRoot.PostAsync(_host, _port, _controllerName, nameof(IServerMessageController.ServerMessages), request, callback);
        }
        #endregion

        #region AddOrUpdateServerMessageAsync
        public void AddOrUpdateServerMessageAsync(ServerMessageData entity, Action<ResponseBase, Exception> callback) {
            DataRequest<ServerMessageData> request = new DataRequest<ServerMessageData>() {
                Data = entity
            };
            RpcRoot.PostAsync(_host, _port, _controllerName, nameof(IServerMessageController.AddOrUpdateServerMessage), request, request, callback);
        }
        #endregion

        #region MarkDeleteServerMessageAsync
        public void MarkDeleteServerMessageAsync(Guid id, Action<ResponseBase, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid>() {
                Data = id
            };
            RpcRoot.PostAsync(_host, _port, _controllerName, nameof(IServerMessageController.MarkDeleteServerMessage), request, request, callback);
        }
        #endregion
    }
}
