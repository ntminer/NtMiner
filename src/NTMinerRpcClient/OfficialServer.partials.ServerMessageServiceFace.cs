using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class OfficialServer {
        public class ServerMessageServiceFace {
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IServerMessageController>();

            private readonly string _host;
            private readonly int _port;

            public ServerMessageServiceFace(string host, int port) {
                _host = host;
                _port = port;
            }

            #region GetServerMessagesAsync
            public void GetServerMessagesAsync(DateTime timestamp, Action<DataResponse<List<ServerMessageData>>, Exception> callback) {
                ServerMessagesRequest request = new ServerMessagesRequest {
                    Timestamp = Timestamp.GetTimestamp(timestamp)
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IServerMessageController.ServerMessages), request, callback);
            }
            #endregion

            #region AddOrUpdateServerMessageAsync
            public void AddOrUpdateServerMessageAsync(ServerMessageData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<ServerMessageData> request = new DataRequest<ServerMessageData>() {
                    Data = entity
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IServerMessageController.AddOrUpdateServerMessage), request, request, callback);
            }
            #endregion

            #region MarkDeleteServerMessageAsync
            public void MarkDeleteServerMessageAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IServerMessageController.MarkDeleteServerMessage), request, request, callback);
            }
            #endregion
        }
    }
}
