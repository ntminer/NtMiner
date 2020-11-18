using NTMiner.Core.MinerServer;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    // 注意该控制器不能重命名
    public class ClientDataController : ApiControllerBase, IClientDataController {
        #region QueryClients
        [Role.User]
        [HttpPost]
        public QueryClientsResponse QueryClients([FromBody]QueryClientsRequest request) {
            return DoQueryClients(request, User);
        }
        #endregion

        internal static QueryClientsResponse DoQueryClients(QueryClientsRequest request, UserData user) {
            if (request == null) {
                return ResponseBase.InvalidInput<QueryClientsResponse>("参数错误");
            }
            request.PagingTrim();
            try {
                var data = WebApiRoot.ClientDataSet.QueryClients(
                    user,
                    request,
                    out int total,
                    out CoinSnapshotData[] latestSnapshots,
                    out int totalOnlineCount,
                    out int totalMiningCount) ?? new List<ClientData>();
                return QueryClientsResponse.Ok(data, total, latestSnapshots, totalMiningCount, totalOnlineCount);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<QueryClientsResponse>(e.Message);
            }
        }

        #region UpdateClient
        [Role.User]
        [HttpPost]
        public ResponseBase UpdateClient([FromBody]UpdateClientRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                var clientData = WebApiRoot.ClientDataSet.GetByObjectId(request.ObjectId);
                if (clientData != null && clientData.IsOwnerBy(User)) {
                    WebApiRoot.ClientDataSet.UpdateClient(request.ObjectId, request.PropertyName, request.Value);
                }
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region UpdateClients
        [Role.User]
        [HttpPost]
        public ResponseBase UpdateClients([FromBody]UpdateClientsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                List<string> toRemoveKeys = new List<string>();
                foreach (var key in request.Values.Keys) {
                    var minerData = WebApiRoot.ClientDataSet.GetByObjectId(key);
                    if (minerData == null && !minerData.IsOwnerBy(User)) {
                        toRemoveKeys.Add(key);
                    }
                }
                foreach (var key in toRemoveKeys) {
                    request.Values.Remove(key);
                }
                WebApiRoot.ClientDataSet.UpdateClients(request.PropertyName, request.Values);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveClients
        [Role.User]
        [HttpPost]
        public ResponseBase RemoveClients([FromBody]MinerIdsRequest request) {
            if (request == null || request.ObjectIds == null) {
                return ResponseBase.InvalidInput("参数错误");
            }

            try {
                foreach (var objectId in request.ObjectIds) {
                    var minerData = WebApiRoot.ClientDataSet.GetByObjectId(objectId);
                    if (minerData != null && (User.IsAdmin() || minerData.IsOwnerBy(User))) {
                        WebApiRoot.ClientDataSet.RemoveByObjectId(objectId);
                    }
                }
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion
    }
}
