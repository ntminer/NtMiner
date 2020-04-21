using NTMiner.Core.MinerServer;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    // 注意该控制器不能重命名
    public class ClientDataController : ApiControllerBase, IClientDataController {
        #region QueryClients
        [HttpPost]
        public QueryClientsResponse QueryClients([FromBody]QueryClientsRequest query) {
            if (query == null) {
                return ResponseBase.InvalidInput<QueryClientsResponse>("参数错误");
            }
            try {
                if (!IsValidUser(query, out QueryClientsResponse response, out UserData user)) {
                    return response;
                }
                var data = WebApiRoot.ClientDataSet.QueryClients(
                    user, 
                    query, 
                    out int total, 
                    out List<CoinSnapshotData> latestSnapshots, 
                    out int totalOnlineCount, 
                    out int totalMiningCount) ?? new List<ClientData>();
                //foreach (var item in data) {
                //    if (!string.IsNullOrEmpty(item.WindowsPassword)) {
                //        item.WindowsPassword = Cryptography.AESHelper.Encrypt(item.WindowsPassword, Cryptography.AESHelper.ConvertToKey(user.Password));
                //    }
                //}
                return QueryClientsResponse.Ok(data, total, latestSnapshots, totalMiningCount, totalOnlineCount);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<QueryClientsResponse>(e.Message);
            }
        }
        #endregion

        #region UpdateClient
        [HttpPost]
        public ResponseBase UpdateClient([FromBody]UpdateClientRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!IsValidUser(request, out ResponseBase response, out UserData user)) {
                    return response;
                }
                var clientData = WebApiRoot.ClientDataSet.GetByObjectId(request.ObjectId);
                if (clientData != null && clientData.IsOwnerBy(user)) {
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
        [HttpPost]
        public ResponseBase UpdateClients([FromBody]UpdateClientsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!IsValidUser(request, out ResponseBase response, out UserData user)) {
                    return response;
                }
                List<string> toRemoveKeys = new List<string>();
                foreach (var key in request.Values.Keys) {
                    var minerData = WebApiRoot.ClientDataSet.GetByObjectId(key);
                    if (minerData == null && !minerData.IsOwnerBy(user)) {
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
        [HttpPost]
        public ResponseBase RemoveClients([FromBody]MinerIdsRequest request) {
            if (request == null || request.ObjectIds == null) {
                return ResponseBase.InvalidInput("参数错误");
            }

            try {
                if (!IsValidUser(request, out ResponseBase response, out UserData user)) {
                    return response;
                }

                foreach (var objectId in request.ObjectIds) {
                    var minerData = WebApiRoot.ClientDataSet.GetByObjectId(objectId);
                    if (minerData != null && minerData.IsOwnerBy(user)) {
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
