using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Service.ServerService {
    public partial class ClientService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IClientController>();

        public ClientService() {
        }

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
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IClientController.QueryClients), request, request, callback);
        }
        #endregion

        #region AddClientsAsync
        public void AddClientsAsync(List<string> clientIps, Action<ResponseBase, Exception> callback) {
            AddClientRequest request = new AddClientRequest() {
                ClientIps = clientIps
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IClientController.AddClients), request, request, callback);
        }
        #endregion

        #region RemoveClientsAsync
        public void RemoveClientsAsync(List<string> objectIds, Action<ResponseBase, Exception> callback) {
            MinerIdsRequest request = new MinerIdsRequest() {
                ObjectIds = objectIds
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IClientController.RemoveClients), request, request, callback);
        }
        #endregion

        #region RefreshClientsAsync
        public void RefreshClientsAsync(List<string> objectIds, Action<DataResponse<List<ClientData>>, Exception> callback) {
            MinerIdsRequest request = new MinerIdsRequest() {
                ObjectIds = objectIds
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IClientController.RefreshClients), request, request, callback);
        }
        #endregion

        #region UpdateClientAsync
        public void UpdateClientAsync(string objectId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
            UpdateClientRequest request = new UpdateClientRequest {
                ObjectId = objectId,
                PropertyName = propertyName,
                Value = value
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IClientController.UpdateClient), request, request, callback);
        }
        #endregion

        #region UpdateClientsAsync
        public void UpdateClientsAsync(string propertyName, Dictionary<string, object> values, Action<ResponseBase, Exception> callback) {
            UpdateClientsRequest request = new UpdateClientsRequest {
                PropertyName = propertyName,
                Values = values
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IClientController.UpdateClients), request, request, callback);
        }
        #endregion
    }
}
