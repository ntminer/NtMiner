using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    // 注意该控制器不能重命名
    public class ClientDataController : ApiControllerBase, IClientDataController {
        #region UpdateClient
        [Role.User]
        [HttpPost]
        public ResponseBase UpdateClient([FromBody]UpdateClientRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                var clientData = AppRoot.ClientDataSet.GetByObjectId(request.ObjectId);
                if (clientData != null && clientData.IsOwnerBy(User)) {
                    AppRoot.ClientDataSet.UpdateClient(request.ObjectId, request.PropertyName, request.Value);
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
                    var minerData = AppRoot.ClientDataSet.GetByObjectId(key);
                    if (minerData == null || !minerData.IsOwnerBy(User)) {
                        toRemoveKeys.Add(key);
                    }
                }
                foreach (var key in toRemoveKeys) {
                    request.Values.Remove(key);
                }
                AppRoot.ClientDataSet.UpdateClients(request.PropertyName, request.Values);
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
                    var minerData = AppRoot.ClientDataSet.GetByObjectId(objectId);
                    if (minerData != null && minerData.IsAdminOrOwnerBy(User)) {
                        AppRoot.ClientDataSet.RemoveByObjectId(objectId);
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
