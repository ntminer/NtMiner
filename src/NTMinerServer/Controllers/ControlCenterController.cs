using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ControlCenterController : ApiController {
        #region LoginControlCenter
        [HttpPost]
        public ResponseBase LoginControlCenter([FromBody]LoginControlCenterRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                IUser user;
                if (!request.IsValid(HostRoot.Current.UserSet, out user, out response)) {
                    return response;
                }
                Write.DevLine($"{request.LoginName}登录");
                VirtualRoot.Happened(new UserLoginedEvent(user));
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region LoadClients
        [HttpPost]
        public LoadClientsResponse LoadClients([FromBody]LoadClientsRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<LoadClientsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region QueryClients
        [HttpPost]
        public QueryClientsResponse QueryClients([FromBody]QueryClientsRequest request) {
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
                    request.PageIndex, 
                    request.PageSize, 
                    request.TimeLimit,
                    request.GroupId, 
                    request.WorkId,
                    request.MinerIp, 
                    request.MinerName, 
                    request.MineState,
                    request.MainCoin, 
                    request.MainCoinPool, 
                    request.MainCoinWallet,
                    request.DualCoin, 
                    request.DualCoinPool, 
                    request.DualCoinWallet,
                    request.Version, 
                    request.Kernel, 
                    out total) ?? new List<ClientData>();
                return QueryClientsResponse.Ok(request.MessageId, data, total);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<QueryClientsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region LatestSnapshots
        [HttpPost]
        public GetCoinSnapshotsResponse LatestSnapshots([FromBody]GetCoinSnapshotsRequest request) {
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
                    out totalMiningCount,
                    out totalOnlineCount) ?? new List<CoinSnapshotData>();
                return GetCoinSnapshotsResponse.Ok(request.MessageId, data, totalMiningCount, totalOnlineCount);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetCoinSnapshotsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region LoadClient
        [HttpPost]
        public LoadClientResponse LoadClient([FromBody]LoadClientRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<LoadClientResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region UpdateClient
        [HttpPost]
        public ResponseBase UpdateClient([FromBody]UpdateClientRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region UpdateClientProperties
        [HttpPost]
        public ResponseBase UpdateClientProperties([FromBody]UpdateClientPropertiesRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.ClientSet.UpdateClientProperties(request.ClientId, request.Values);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region MinerGroups
        [HttpPost]
        public GetMinerGroupsResponse MinerGroups([FromBody]MinerGroupsRequest request) {
            try {
                var data = HostRoot.Current.MinerGroupSet.GetMinerGroups();
                return GetMinerGroupsResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetMinerGroupsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMinerGroup
        [HttpPost]
        public ResponseBase AddOrUpdateMinerGroup([FromBody]AddOrUpdateMinerGroupRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveMinerGroup
        [HttpPost]
        public ResponseBase RemoveMinerGroup([FromBody]RemoveMinerGroupRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMineWork
        [HttpPost]
        public ResponseBase AddOrUpdateMineWork([FromBody]AddOrUpdateMineWorkRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveMineWork
        [HttpPost]
        public ResponseBase RemoveMineWork([FromBody]RemoveMineWorkRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetMinerProfileProperty
        [HttpPost]
        public ResponseBase SetMinerProfileProperty([FromBody]SetMinerProfilePropertyRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetCoinProfileProperty
        [HttpPost]
        public ResponseBase SetCoinProfileProperty([FromBody]SetCoinProfilePropertyRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetPoolProfileProperty
        [HttpPost]
        public ResponseBase SetPoolProfileProperty([FromBody]SetPoolProfilePropertyRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetCoinKernelProfileProperty
        [HttpPost]
        public ResponseBase SetCoinKernelProfileProperty([FromBody]SetCoinKernelProfilePropertyRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region Wallets
        [HttpPost]
        public GetWalletsResponse Wallets([FromBody]WalletsRequest request) {
            try {
                var data = HostRoot.Current.WalletSet.GetWallets();
                return GetWalletsResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetWalletsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateWallet
        [HttpPost]
        public ResponseBase AddOrUpdateWallet([FromBody]AddOrUpdateWalletRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveWallet
        [HttpPost]
        public ResponseBase RemoveWallet([FromBody]RemoveWalletRequest request) {
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
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region CalcConfigs
        [HttpPost]
        public GetCalcConfigsResponse CalcConfigs([FromBody]CalcConfigsRequest request) {
            try {
                var data = HostRoot.Current.CalcConfigSet.GetCalcConfigs();
                return GetCalcConfigsResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetCalcConfigsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SaveCalcConfigs
        [HttpPost]
        public ResponseBase SaveCalcConfigs([FromBody]SaveCalcConfigsRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.CalcConfigSet.SaveCalcConfigs(request.Data);
                Write.DevLine("SaveCalcConfigs");
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region ColumnsShows
        [HttpPost]
        public GetColumnsShowsResponse ColumnsShows([FromBody]ColumnsShowsRequest request) {
            try {
                var data = HostRoot.Current.ColumnsShowSet.GetColumnsShows();
                return GetColumnsShowsResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetColumnsShowsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateColumnsShow
        [HttpPost]
        public ResponseBase AddOrUpdateColumnsShow([FromBody]AddOrUpdateColumnsShowRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.ColumnsShowSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveColumnsShow
        [HttpPost]
        public ResponseBase RemoveColumnsShow([FromBody]RemoveColumnsShowRequest request) {
            if (request == null || request.ColumnsShowId == Guid.Empty) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.ColumnsShowSet.Remove(request.ColumnsShowId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion
    }
}
