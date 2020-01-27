using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ClientController : ApiControllerBase, IClientController {
        #region QueryClients
        [HttpPost]
        public QueryClientsResponse QueryClients([FromBody]QueryClientsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<QueryClientsResponse>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out QueryClientsResponse response)) {
                    return response;
                }
                var data = HostRoot.Instance.ClientSet.QueryClients(
                               request.PageIndex,
                                request.PageSize,
                                request.GroupId,
                                request.WorkId,
                                request.MinerIp,
                                request.MinerName,
                                request.MineState,
                                request.Coin,
                                request.Pool,
                                request.Wallet,
                                request.Version,
                                request.Kernel,
                                out int total,
                                out int miningCount) ?? new List<ClientData>();
                return QueryClientsResponse.Ok(data, total, miningCount);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<QueryClientsResponse>(e.Message);
            }
        }
        #endregion

        #region RefreshClients

        public DataResponse<List<ClientData>> RefreshClients([FromBody]MinerIdsRequest request) {
            if (request == null || request.ObjectIds == null) {
                return ResponseBase.InvalidInput<DataResponse<List<ClientData>>>("参数错误");
            }
            if (!HostRoot.Instance.HostConfig.IsPull) {
                return ResponseBase.InvalidInput<DataResponse<List<ClientData>>>("服务端配置为不支持刷新");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out DataResponse<List<ClientData>> response)) {
                    return response;
                }

                var data = HostRoot.Instance.ClientSet.RefreshClients(request.ObjectIds);
                return DataResponse<List<ClientData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<ClientData>>>(e.Message);
            }
        }
        #endregion

        #region AddClients

        [HttpPost]
        public ResponseBase AddClients([FromBody]AddClientRequest request) {
            if (request == null || request.ClientIps == null) {
                return ResponseBase.InvalidInput("参数错误");
            }

            if (request.ClientIps.Count > 101) {
                return ResponseBase.InvalidInput("最多支持一次添加101个IP");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }

                if (request.ClientIps.Any(a => !IPAddress.TryParse(a, out IPAddress ip))) {
                    return ResponseBase.InvalidInput("IP格式不正确");
                }

                foreach (var clientIp in request.ClientIps) {
                    ClientData clientData = HostRoot.Instance.ClientSet.AsEnumerable().FirstOrDefault(a => a.MinerIp == clientIp);
                    if (clientData != null) {
                        continue;
                    }
                    HostRoot.Instance.ClientSet.AddMiner(clientIp);
                }
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
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }

                foreach (var objectId in request.ObjectIds) {
                    HostRoot.Instance.ClientSet.Remove(objectId);
                }
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
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
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.ClientSet.UpdateClient(request.ObjectId, request.PropertyName, request.Value);
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
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.ClientSet.UpdateClients(request.PropertyName, request.Values);
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
