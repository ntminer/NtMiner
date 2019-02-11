using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public class ControlCenterServiceFace {
            public static readonly ControlCenterServiceFace Instance = new ControlCenterServiceFace();

            private ControlCenterServiceFace() {
            }

            #region LoginAsync
            public void LoginAsync(string loginName, string password, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        LoginControlCenterRequest request = new LoginControlCenterRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = loginName,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(password);
                        ResponseBase response = Request<ResponseBase>("ControlCenter", "LoginControlCenter", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region LoadClientsAsync
            public void LoadClientsAsync(List<Guid> clientIds, Action<LoadClientsResponse> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        LoadClientsRequest request = new LoadClientsRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            ClientIds = clientIds,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        LoadClientsResponse response = Request<LoadClientsResponse>("ControlCenter", "LoadClients", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region GetLatestSnapshotsAsync
            public void GetLatestSnapshotsAsync(
                int limit,
                List<string> coinCodes,
                Action<GetCoinSnapshotsResponse> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        GetCoinSnapshotsRequest request = new GetCoinSnapshotsRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            Limit = limit,
                            CoinCodes = coinCodes,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        GetCoinSnapshotsResponse response = Request<GetCoinSnapshotsResponse>("ControlCenter", "LatestSnapshots", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region LoadClientAsync
            public void LoadClientAsync(Guid clientId, Action<LoadClientResponse> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        LoadClientRequest request = new LoadClientRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            ClientId = clientId,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        LoadClientResponse response = Request<LoadClientResponse>("ControlCenter", "LoadClient", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region QueryClientsAsync
            public void QueryClientsAsync(
                int pageIndex,
                int pageSize,
                Guid? mineWorkId,
                string minerIp,
                string minerName,
                MineStatus mineState,
                string mainCoin,
                string mainCoinPool,
                string mainCoinWallet,
                string dualCoin,
                string dualCoinPool,
                string dualCoinWallet,
                string version,
                string kernel,
                Action<QueryClientsResponse> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        var request = new QueryClientsRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            PageIndex = pageIndex,
                            PageSize = pageSize,
                            MineWorkId = mineWorkId,
                            MinerIp = minerIp,
                            MinerName = minerName,
                            MineState = mineState,
                            MainCoin = mainCoin,
                            MainCoinPool = mainCoinPool,
                            MainCoinWallet = mainCoinWallet,
                            DualCoin = dualCoin,
                            DualCoinPool = dualCoinPool,
                            DualCoinWallet = dualCoinWallet,
                            Version = version,
                            Kernel = kernel,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        QueryClientsResponse response = Request<QueryClientsResponse>("ControlCenter", "QueryClients", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region UpdateClientAsync
            public void UpdateClientAsync(Guid clientId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        UpdateClientRequest request = new UpdateClientRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            ClientId = clientId,
                            PropertyName = propertyName,
                            Value = value,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        QueryClientsResponse response = Request<QueryClientsResponse>("ControlCenter", "UpdateClient", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region GetMinerGroups
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="messageId"></param>
            /// <returns></returns>
            public GetMinerGroupsResponse GetMinerGroups(Guid messageId) {
                try {
                    MinerGroupsRequest request = new MinerGroupsRequest {
                        MessageId = Guid.NewGuid()
                    };
                    GetMinerGroupsResponse response = Request<GetMinerGroupsResponse>("ControlCenter", "MinerGroups", request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region AddOrUpdateMinerGroupAsync
            public void AddOrUpdateMinerGroupAsync(MinerGroupData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        entity.ModifiedOn = DateTime.Now;
                        AddOrUpdateMinerGroupRequest request = new AddOrUpdateMinerGroupRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            Data = entity,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("ControlCenter", "AddOrUpdateMinerGroup", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region RemoveMinerGroupAsync
            public void RemoveMinerGroupAsync(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RemoveMinerGroupRequest request = new RemoveMinerGroupRequest() {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("ControlCenter", "RemoveMinerGroup", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region AddOrUpdateMineWorkAsync
            public void AddOrUpdateMineWorkAsync(MineWorkData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        entity.ModifiedOn = DateTime.Now;
                        AddOrUpdateMineWorkRequest request = new AddOrUpdateMineWorkRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            Timestamp = DateTime.Now,
                            Data = entity
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("ControlCenter", "AddOrUpdateMineWork", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region RemoveMineWorkAsync
            public void RemoveMineWorkAsync(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RemoveMineWorkRequest request = new RemoveMineWorkRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            Timestamp = DateTime.Now,
                            MineWorkId = id
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("ControlCenter", "RemoveMineWork", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region SetMinerProfilePropertyAsync
            public void SetMinerProfilePropertyAsync(Guid workId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        SetMinerProfilePropertyRequest request = new SetMinerProfilePropertyRequest() {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            PropertyName = propertyName,
                            Value = value,
                            Timestamp = DateTime.Now,
                            WorkId = workId
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("ControlCenter", "SetMinerProfileProperty", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region SetCoinProfilePropertyAsync
            public void SetCoinProfilePropertyAsync(Guid workId, Guid coinId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        SetCoinProfilePropertyRequest request = new SetCoinProfilePropertyRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            CoinId = coinId,
                            WorkId = workId,
                            PropertyName = propertyName,
                            Value = value,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("ControlCenter", "SetCoinProfileProperty", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region SetPoolProfilePropertyAsync
            public void SetPoolProfilePropertyAsync(Guid workId, Guid poolId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        SetPoolProfilePropertyRequest request = new SetPoolProfilePropertyRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            PoolId = poolId,
                            WorkId = workId,
                            PropertyName = propertyName,
                            Value = value,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("ControlCenter", "SetPoolProfileProperty", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region SetCoinKernelProfilePropertyAsync
            public void SetCoinKernelProfilePropertyAsync(Guid workId, Guid coinKernelId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        SetCoinKernelProfilePropertyRequest request = new SetCoinKernelProfilePropertyRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            CoinKernelId = coinKernelId,
                            PropertyName = propertyName,
                            Value = value,
                            Timestamp = DateTime.Now,
                            WorkId = workId
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("ControlCenter", "SetCoinKernelProfileProperty", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region GetWallets
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public GetWalletsResponse GetWallets() {
                try {
                    WalletsRequest request = new WalletsRequest {
                        MessageId = Guid.NewGuid()
                    };
                    GetWalletsResponse response = Request<GetWalletsResponse>("ControlCenter", "Wallets", request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region AddOrUpdateWalletAsync
            public void AddOrUpdateWalletAsync(WalletData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        entity.ModifiedOn = DateTime.Now;
                        AddOrUpdateWalletRequest request = new AddOrUpdateWalletRequest {
                            LoginName = LoginName,
                            MessageId = Guid.NewGuid(),
                            Timestamp = DateTime.Now,
                            Data = entity
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("ControlCenter", "AddOrUpdateWallet", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region RemoveWalletAsync
            public void RemoveWalletAsync(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RemoveWalletRequest request = new RemoveWalletRequest {
                            MessageId = Guid.NewGuid(),
                            LoginName = LoginName,
                            Timestamp = DateTime.Now,
                            WalletId = id
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("ControlCenter", "RemoveWallet", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region GetCalcConfigs
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public GetCalcConfigsResponse GetCalcConfigs() {
                try {
                    CalcConfigsRequest request = new CalcConfigsRequest {
                        MessageId = Guid.NewGuid()
                    };
                    GetCalcConfigsResponse response = Request<GetCalcConfigsResponse>("ControlCenter", "CalcConfigs", request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region SaveCalcConfigsAsync
            public void SaveCalcConfigsAsync(List<CalcConfigData> configs, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        if (configs == null || configs.Count == 0) {
                            return;
                        }
                        SaveCalcConfigsRequest request = new SaveCalcConfigsRequest {
                            Data = configs,
                            LoginName = LoginName,
                            MessageId = Guid.NewGuid(),
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("ControlCenter", "SaveCalcConfigs", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion
        }
    }
}