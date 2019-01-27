using LiteDB;
using NTMiner.Data.Impl;
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
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.Forbidden(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId, "登录名不存在");
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "密码错误");
                }
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        public ResponseBase SetServerJsonVersion(SetServerJsonVersionRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
                }
                using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                    var col = db.GetCollection<HostConfigData>();
                    var hostConfigData = col.FindOne(Query.All());
                    if (hostConfigData != null) {
                        hostConfigData.ServerJsonVersion = Global.GetTimestamp();
                        HostRoot.Current.HostConfig.ServerJsonVersion = hostConfigData.ServerJsonVersion;
                        col.Update(hostConfigData);
                        Global.DebugLine("SetServerJsonVersion " + hostConfigData.ServerJsonVersion);
                        return ResponseBase.Ok(request.MessageId);
                    }
                    return ResponseBase.ServerError(request.MessageId, "HostConfigData记录不存在");
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        #region LoadClients
        public LoadClientsResponse LoadClients(LoadClientsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<LoadClientsResponse>(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput<LoadClientsResponse>(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden<LoadClientsResponse>(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired<LoadClientsResponse>(request.MessageId);
                }
                if (request.ClientIds == null || request.ClientIds.Count == 0) {
                    return ResponseBase.InvalidInput<LoadClientsResponse>(request.MessageId, "clientIds为空");
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden<LoadClientsResponse>(request.MessageId, "签名验证未通过");
                }
                var data = HostRoot.Current.ClientSet.LoadClients(request.ClientIds) ?? new List<ClientData>();
                return new LoadClientsResponse(data);
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
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput<QueryClientsResponse>(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden<QueryClientsResponse>(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired<QueryClientsResponse>(request.MessageId);
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden<QueryClientsResponse>(request.MessageId, "签名验证未通过");
                }
                int total;
                var data = HostRoot.Current.ClientSet.QueryClients(
                    request.PageIndex, request.PageSize, request.MineWorkId,
                    request.MinerIp, request.MinerName, request.MineState,
                    request.MainCoin, request.MainCoinPool, request.MainCoinWallet,
                    request.DualCoin, request.DualCoinPool, request.DualCoinWallet,
                    request.Version, request.Kernel, out total) ?? new List<ClientData>();
                return new QueryClientsResponse(data) {
                    Total = total
                };
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
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput<GetCoinSnapshotsResponse>(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden<GetCoinSnapshotsResponse>(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired<GetCoinSnapshotsResponse>(request.MessageId);
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden<GetCoinSnapshotsResponse>(request.MessageId, "签名验证未通过");
                }
                int totalMiningCount;
                int totalOnlineCount;
                List<CoinSnapshotData> data = HostRoot.Current.CoinSnapshotSet.GetLatestSnapshots(
                    request.Limit,
                    request.CoinCodes,
                    out totalMiningCount,
                    out totalOnlineCount) ?? new List<CoinSnapshotData>();
                return new GetCoinSnapshotsResponse(data) {
                    TotalMiningCount = totalMiningCount,
                    TotalOnlineCount = totalOnlineCount
                };
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetCoinSnapshotsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region LoadClient
        public LoadClientResponse LoadClient(LoadClientRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<LoadClientResponse>(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput<LoadClientResponse>(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden<LoadClientResponse>(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired<LoadClientResponse>(request.MessageId);
                }
                if (request.ClientId == Guid.Empty) {
                    return ResponseBase.InvalidInput<LoadClientResponse>(request.MessageId, "clientId为空");
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden<LoadClientResponse>(request.MessageId, "签名验证未通过");
                }
                var data = HostRoot.Current.ClientSet.LoadClient(request.MessageId);
                return new LoadClientResponse {
                    Data = data
                };
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
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
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
                return new GetMinerGroupsResponse(data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetMinerGroupsResponse>(messageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMinerGroup
        public ResponseBase AddOrUpdateMinerGroup(AddOrUpdateMinerGroupRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (request.Data == null) {
                    return ResponseBase.InvalidInput(request.MessageId);
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
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
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (request.MinerGroupId == Guid.Empty) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的id不存在");
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
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
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (request.Data == null) {
                    return ResponseBase.InvalidInput(request.MessageId);
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
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
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (request.MineWorkId == Guid.Empty) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的id不存在");
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
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
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (!HostRoot.Current.MineWorkSet.Contains(request.WorkId)) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的workId不存在");
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
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
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (!HostRoot.Current.MineWorkSet.Contains(request.WorkId)) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的workId不存在");
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
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
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (!HostRoot.Current.MineWorkSet.Contains(request.WorkId)) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的workId不存在");
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
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
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (!HostRoot.Current.MineWorkSet.Contains(request.WorkId)) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的workId不存在");
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
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
                return new GetWalletsResponse(data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetWalletsResponse>(messageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateWallet
        public ResponseBase AddOrUpdateWallet(AddOrUpdateWalletRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (request.Data == null) {
                    return ResponseBase.InvalidInput(request.MessageId);
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
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
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (request.WalletId == Guid.Empty) {
                    return ResponseBase.InvalidInput(request.MessageId, "WalletId为空");
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
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
                return new GetCalcConfigsResponse(data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetCalcConfigsResponse>(messageId, e.Message);
            }
        }
        #endregion

        #region SaveCalcConfigs
        public ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (request.Data == null) {
                    return ResponseBase.InvalidInput(request.MessageId);
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
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
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
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
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
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
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
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
