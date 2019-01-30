using NTMiner.ServiceContracts;
using NTMiner.ServiceContracts;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;

namespace NTMiner.Services {
    public class ControlCenterServiceImpl : IControlCenterService {
        public ControlCenterServiceImpl() { }

        #region LoginControlCenter
        public ResponseBase LoginControlCenter(LoginControlCenterRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region LoadClients
        public LoadClientsResponse LoadClients(LoadClientsRequest request) {
            if (request == null || request.ClientIds == null || request.ClientIds.Count == 0) {
                return ResponseBase.InvalidInput<LoadClientsResponse>(Guid.Empty, "参数错误");
            }
            try {
                LoadClientsResponse response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                var data = HostRoot.Current.ClientSet.LoadClients(request.ClientIds) ?? new List<ClientData>();
                return LoadClientsResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<LoadClientsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region QueryClients
        public QueryClientsResponse QueryClients(QueryClientsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<QueryClientsResponse>(Guid.Empty, "参数错误");
            }
            try {
                QueryClientsResponse response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                int total;
                var data = HostRoot.Current.ClientSet.QueryClients(
                    request.PageIndex, request.PageSize, request.MineWorkId,
                    request.MinerIp, request.MinerName, request.MineState,
                    request.MainCoin, request.MainCoinPool, request.MainCoinWallet,
                    request.DualCoin, request.DualCoinPool, request.DualCoinWallet,
                    request.Version, request.Kernel, out total) ?? new List<ClientData>();
                return QueryClientsResponse.Ok(request.MessageId, data, total);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<QueryClientsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region GetCoinSnapshots
        public GetCoinSnapshotsResponse GetLatestSnapshots(GetCoinSnapshotsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<GetCoinSnapshotsResponse>(Guid.Empty, "参数错误");
            }
            try {
                GetCoinSnapshotsResponse response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                int totalMiningCount;
                int totalOnlineCount;
                List<CoinSnapshotData> data = HostRoot.Current.CoinSnapshotSet.GetLatestSnapshots(
                    request.Limit,
                    request.CoinCodes,
                    out totalMiningCount,
                    out totalOnlineCount) ?? new List<CoinSnapshotData>();
                return GetCoinSnapshotsResponse.Ok(request.MessageId, data, totalMiningCount, totalOnlineCount);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetCoinSnapshotsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region LoadClient
        public LoadClientResponse LoadClient(LoadClientRequest request) {
            if (request == null || request.ClientId == Guid.Empty) {
                return ResponseBase.InvalidInput<LoadClientResponse>(Guid.Empty, "参数错误");
            }
            try {
                LoadClientResponse response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                var data = HostRoot.Current.ClientSet.LoadClient(request.MessageId);
                return LoadClientResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<LoadClientResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region UpdateClient
        public ResponseBase UpdateClient(UpdateClientRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.ClientSet.UpdateClient(request.ClientId, request.PropertyName, request.Value);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region GetMinerGroups
        public GetMinerGroupsResponse GetMinerGroups(Guid messageId) {
            try {
                var data = HostRoot.Current.MinerGroupSet.GetMinerGroups();
                return GetMinerGroupsResponse.Ok(messageId, data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetMinerGroupsResponse>(messageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMinerGroup
        public ResponseBase AddOrUpdateMinerGroup(AddOrUpdateMinerGroupRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.MinerGroupSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveMinerGroup
        public ResponseBase RemoveMinerGroup(RemoveMinerGroupRequest request) {
            if (request == null || request.MinerGroupId == Guid.Empty) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.MinerGroupSet.Remove(request.MinerGroupId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMineWork
        public ResponseBase AddOrUpdateMineWork(AddOrUpdateMineWorkRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.MineWorkSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveMineWork
        public ResponseBase RemoveMineWork(RemoveMineWorkRequest request) {
            if (request == null || request.MineWorkId == Guid.Empty) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.MineWorkSet.Remove(request.MineWorkId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetMinerProfileProperty
        public ResponseBase SetMinerProfileProperty(SetMinerProfilePropertyRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                if (!HostRoot.Current.MineWorkSet.Contains(request.WorkId)) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的workId不存在");
                }
                HostRoot.Current.MineProfileManager.SetMinerProfileProperty(request.WorkId, request.PropertyName, request.Value);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetCoinProfileProperty
        public ResponseBase SetCoinProfileProperty(SetCoinProfilePropertyRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                if (!HostRoot.Current.MineWorkSet.Contains(request.WorkId)) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的workId不存在");
                }
                HostRoot.Current.MineProfileManager.SetCoinProfileProperty(request.WorkId, request.CoinId, request.PropertyName, request.Value);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetPoolProfileProperty
        public ResponseBase SetPoolProfileProperty(SetPoolProfilePropertyRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                if (!HostRoot.Current.MineWorkSet.Contains(request.WorkId)) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的workId不存在");
                }
                HostRoot.Current.MineProfileManager.SetPoolProfileProperty(request.WorkId, request.PoolId, request.PropertyName, request.Value);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetCoinKernelProfileProperty
        public ResponseBase SetCoinKernelProfileProperty(SetCoinKernelProfilePropertyRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                if (!HostRoot.Current.MineWorkSet.Contains(request.WorkId)) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的workId不存在");
                }
                HostRoot.Current.MineProfileManager.SetCoinKernelProfileProperty(request.WorkId, request.CoinKernelId, request.PropertyName, request.Value);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region GetWallets
        public GetWalletsResponse GetWallets(Guid messageId) {
            try {
                var data = HostRoot.Current.WalletSet.GetWallets();
                return GetWalletsResponse.Ok(messageId, data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetWalletsResponse>(messageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateWallet
        public ResponseBase AddOrUpdateWallet(AddOrUpdateWalletRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.WalletSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveWallet
        public ResponseBase RemoveWallet(RemoveWalletRequest request) {
            if (request == null || request.WalletId == Guid.Empty) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.WalletSet.Remove(request.WalletId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region GetCalcConfigs
        public GetCalcConfigsResponse GetCalcConfigs(Guid messageId) {
            try {
                var data = HostRoot.Current.CalcConfigSet.GetCalcConfigs();
                return GetCalcConfigsResponse.Ok(messageId, data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetCalcConfigsResponse>(messageId, e.Message);
            }
        }
        #endregion

        #region SaveCalcConfigs
        public ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.CalcConfigSet.SaveCalcConfigs(request.Data);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region StartMine
        public ResponseBase StartMine(StartMineRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                throw new NotImplementedException();
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region StopMine
        public ResponseBase StopMine(StopMineRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                throw new NotImplementedException();
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetClientMinerProfileProperty
        public ResponseBase SetClientMinerProfileProperty(SetClientMinerProfilePropertyRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                throw new NotImplementedException();
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        public void Dispose() {
            // nothing need todo
        }
    }
}
