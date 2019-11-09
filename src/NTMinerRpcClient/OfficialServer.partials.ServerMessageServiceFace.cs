using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static partial class OfficialServer {
        public class ServerMessageServiceFace {
            public static readonly ServerMessageServiceFace Instance = new ServerMessageServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IServerMessageController>();

            private ServerMessageServiceFace() {
            }

            #region GetServerMessagesAsync
            private DateTime _lastGetServerMessageOn;
            public void GetServerMessagesAsync(DateTime timestamp, Action<DataResponse<List<ServerMessageData>>, Exception> callback) {
                // 一种防御，简化消费者逻辑
                if (_lastGetServerMessageOn.AddSeconds(1) >= DateTime.Now) {
                    callback?.Invoke(new DataResponse<List<ServerMessageData>>(), null);
                    return;
                }
                _lastGetServerMessageOn = DateTime.Now;
                ServerMessagesRequest request = new ServerMessagesRequest {
                    Timestamp = Timestamp.GetTimestamp(timestamp)
                };
                PostAsync(SControllerName, nameof(IServerMessageController.ServerMessages), null, request, callback);
            }
            #endregion

            #region AddOrUpdateServerMessageAsync
            public void AddOrUpdateServerMessageAsync(ServerMessageData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<ServerMessageData> request = new DataRequest<ServerMessageData>() {
                    Data = entity
                };
                PostAsync(SControllerName, nameof(IServerMessageController.AddOrUpdateServerMessage), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region MarkDeleteServerMessageAsync
            public void MarkDeleteServerMessageAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                PostAsync(SControllerName, nameof(IServerMessageController.MarkDeleteServerMessage), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion
        }
    }
}
