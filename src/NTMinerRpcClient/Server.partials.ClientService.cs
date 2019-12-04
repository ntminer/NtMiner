using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class Server {
        public partial class ClientServiceFace {
            public static readonly ClientServiceFace Instance = new ClientServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IClientController>();

            private ClientServiceFace() { }

            #region QueryClientsAsync
            public void QueryClientsAsync(
                int pageIndex,
                int pageSize,
                Guid? groupId,
                Guid? workId,
                string minerIp,
                string minerName,
                MineStatus mineState,
                string coin,
                string pool,
                string wallet,
                string version,
                string kernel,
                Action<QueryClientsResponse, Exception> callback) {
                var request = new QueryClientsRequest {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    GroupId = groupId,
                    WorkId = workId,
                    MinerIp = minerIp,
                    MinerName = minerName,
                    MineState = mineState,
                    Coin = coin,
                    Pool = pool,
                    Wallet = wallet,
                    Version = version,
                    Kernel = kernel
                };
                PostAsync(SControllerName, nameof(IClientController.QueryClients), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region AddClientsAsync
            public void AddClientsAsync(List<string> clientIps, Action<ResponseBase, Exception> callback) {
                AddClientRequest request = new AddClientRequest() {
                    ClientIps = clientIps
                };
                PostAsync(SControllerName, nameof(IClientController.AddClients), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region RemoveClientsAsync
            public void RemoveClientsAsync(List<string> objectIds, Action<ResponseBase, Exception> callback) {
                MinerIdsRequest request = new MinerIdsRequest() {
                    ObjectIds = objectIds
                };
                PostAsync(SControllerName, nameof(IClientController.RemoveClients), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region RefreshClientsAsync
            public void RefreshClientsAsync(List<string> objectIds, Action<DataResponse<List<ClientData>>, Exception> callback) {
                MinerIdsRequest request = new MinerIdsRequest() {
                    ObjectIds = objectIds
                };
                PostAsync(SControllerName, nameof(IClientController.RefreshClients), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region UpdateClientAsync
            public void UpdateClientAsync(string objectId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                UpdateClientRequest request = new UpdateClientRequest {
                    ObjectId = objectId,
                    PropertyName = propertyName,
                    Value = value
                };
                PostAsync(SControllerName, nameof(IClientController.UpdateClient), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region UpdateClientsAsync
            public void UpdateClientsAsync(string propertyName, Dictionary<string, object> values, Action<ResponseBase, Exception> callback) {
                UpdateClientsRequest request = new UpdateClientsRequest {
                    PropertyName = propertyName,
                    Values = values
                };
                PostAsync(SControllerName, nameof(IClientController.UpdateClients), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion
        }
    }
}
