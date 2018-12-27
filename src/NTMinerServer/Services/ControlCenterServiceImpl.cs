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
                return LoadClientsResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region LoadClients
        public LoadClientsResponse LoadClients(LoadClientsRequest request) {
            if (request == null) {
                return LoadClientsResponse.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return LoadClientsResponse.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return LoadClientsResponse.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return LoadClientsResponse.Expired(request.MessageId);
                }
                if (request.ClientIds == null || request.ClientIds.Count == 0) {
                    return LoadClientsResponse.InvalidInput(request.MessageId, "clientIds为空");
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return LoadClientsResponse.Forbidden(request.MessageId, "签名验证未通过");
                }
                var data = HostRoot.Current.ClientSet.LoadClients(request.ClientIds) ?? new List<ClientData>();
                return new LoadClientsResponse(data);
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
                return LoadClientsResponse.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region QueryClients
        public QueryClientsResponse QueryClients(QueryClientsRequest request) {
            if (request == null) {
                return QueryClientsResponse.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return QueryClientsResponse.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return QueryClientsResponse.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return QueryClientsResponse.Expired(request.MessageId);
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return QueryClientsResponse.Forbidden(request.MessageId, "签名验证未通过");
                }
                int total;
                var data = HostRoot.Current.ClientSet.QueryClients(
                    request.PageIndex, request.PageSize, request.MineWorkId,
                    request.MinerIp, request.MinerName, request.MineState,
                    request.MainCoin, request.MainCoinPool, request.MainCoinWallet,
                    request.DualCoin, request.DualCoinPool, request.DualCoinWallet, out total) ?? new List<ClientData>();
                return new QueryClientsResponse(data) {
                    Total = total
                };
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
                return QueryClientsResponse.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region GetCoinSnapshots
        public GetCoinSnapshotsResponse GetLatestSnapshots(GetCoinSnapshotsRequest request) {
            if (request == null) {
                return GetCoinSnapshotsResponse.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return GetCoinSnapshotsResponse.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return GetCoinSnapshotsResponse.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return GetCoinSnapshotsResponse.Expired(request.MessageId);
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return GetCoinSnapshotsResponse.Forbidden(request.MessageId, "签名验证未通过");
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
                Global.Logger.Error(e.Message, e);
                return GetCoinSnapshotsResponse.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region LoadClient
        public LoadClientResponse LoadClient(LoadClientRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return LoadClientResponse.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return LoadClientResponse.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return LoadClientResponse.Expired(request.MessageId);
                }
                if (request.ClientId == Guid.Empty) {
                    return LoadClientResponse.InvalidInput(request.MessageId, "clientId为空");
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return LoadClientResponse.Forbidden(request.MessageId, "签名验证未通过");
                }
                var data = HostRoot.Current.ClientSet.LoadClient(request.MessageId);
                return new LoadClientResponse {
                    Data = data
                };
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
                return LoadClientResponse.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region UpdateClient
        public ResponseBase UpdateClient(UpdateClientRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
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
                Global.Logger.Error(e.Message, e);
                return GetMinerGroupsResponse.ServerError(messageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMinerGroup
        public ResponseBase AddOrUpdateMinerGroup(AddOrUpdateMinerGroupRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveMinerGroup
        public ResponseBase RemoveMinerGroup(RemoveMinerGroupRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMineWork
        public ResponseBase AddOrUpdateMineWork(AddOrUpdateMineWorkRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveMineWork
        public ResponseBase RemoveMineWork(RemoveMineWorkRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetMinerProfileProperty
        public ResponseBase SetMinerProfileProperty(SetMinerProfilePropertyRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetCoinProfileProperty
        public ResponseBase SetCoinProfileProperty(SetCoinProfilePropertyRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetCoinKernelProfileProperty
        public ResponseBase SetCoinKernelProfileProperty(SetCoinKernelProfilePropertyRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
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
                Global.Logger.Error(e.Message, e);
                return GetWalletsResponse.ServerError(messageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateWallet
        public ResponseBase AddOrUpdateWallet(AddOrUpdateWalletRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveWallet
        public ResponseBase RemoveWallet(RemoveWalletRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
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
                Global.Logger.Error(e.Message, e);
                return GetCalcConfigsResponse.ServerError(messageId, e.Message);
            }
        }
        #endregion

        #region SaveCalcConfigs
        public ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region StartMine
        public ResponseBase StartMine(StartMineRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region StopMine
        public ResponseBase StopMine(StopMineRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetClientMinerProfileProperty
        public ResponseBase SetClientMinerProfileProperty(SetClientMinerProfilePropertyRequest request) {
            if (request == null) {
                return LoadClientResponse.InvalidInput(Guid.Empty, "参数错误");
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
                Global.Logger.Error(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        public void Dispose() {
            // nothing need todo
        }
    }
}
