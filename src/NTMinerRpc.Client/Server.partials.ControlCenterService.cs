using NTMiner.MinerServer;
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
                    Guid messageId = Guid.NewGuid();
                    LoginControlCenterRequest request = new LoginControlCenterRequest {
                        MessageId = messageId,
                        LoginName = loginName,
                        Timestamp = DateTime.Now
                    };
                    request.SignIt(password);
                    ResponseBase response = Request<ResponseBase>("ControlCenter", "LoginControlCenter", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region LoadClientsAsync
            public void LoadClientsAsync(List<Guid> clientIds, Action<LoadClientsResponse> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    LoadClientsRequest request = new LoadClientsRequest {
                        MessageId = messageId,
                        LoginName = LoginName,
                        ClientIds = clientIds,
                        Timestamp = DateTime.Now
                    };
                    request.SignIt(PasswordSha1);
                    LoadClientsResponse response = Request<LoadClientsResponse>("ControlCenter", "LoadClients", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region GetLatestSnapshotsAsync
            public void GetLatestSnapshotsAsync(
                int limit,
                List<string> coinCodes,
                Action<GetCoinSnapshotsResponse> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    GetCoinSnapshotsRequest request = new GetCoinSnapshotsRequest {
                        MessageId = messageId,
                        LoginName = LoginName,
                        Limit = limit,
                        CoinCodes = coinCodes,
                        Timestamp = DateTime.Now
                    };
                    request.SignIt(PasswordSha1);
                    GetCoinSnapshotsResponse response = Request<GetCoinSnapshotsResponse>("ControlCenter", "LatestSnapshots", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region LoadClientAsync
            public void LoadClientAsync(Guid clientId, Action<LoadClientResponse> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    LoadClientRequest request = new LoadClientRequest {
                        MessageId = messageId,
                        LoginName = LoginName,
                        ClientId = clientId,
                        Timestamp = DateTime.Now
                    };
                    request.SignIt(PasswordSha1);
                    LoadClientResponse response = Request<LoadClientResponse>("ControlCenter", "LoadClient", request);
                    callback?.Invoke(response);
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
                    Guid messageId = Guid.NewGuid();
                    var request = new QueryClientsRequest {
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
                });
            }
            #endregion

            #region UpdateClientAsync
            public void UpdateClientAsync(Guid clientId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    UpdateClientRequest request = new UpdateClientRequest {
                        MessageId = messageId,
                        LoginName = LoginName,
                        ClientId = clientId,
                        PropertyName = propertyName,
                        Value = value,
                        Timestamp = DateTime.Now
                    };
                    request.SignIt(PasswordSha1);
                    QueryClientsResponse response = Request<QueryClientsResponse>("ControlCenter", "UpdateClient", request);
                    callback?.Invoke(response);
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
                MinerGroupsRequest request = new MinerGroupsRequest {
                    MessageId = Guid.NewGuid()
                };
                GetMinerGroupsResponse response = Request<GetMinerGroupsResponse>("ControlCenter", "MinerGroups", request);
                return response;
            }
            #endregion

            #region AddOrUpdateMinerGroupAsync
            public void AddOrUpdateMinerGroupAsync(MinerGroupData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    entity.ModifiedOn = DateTime.Now;
                    AddOrUpdateMinerGroupRequest request = new AddOrUpdateMinerGroupRequest {
                        MessageId = messageId,
                        LoginName = LoginName,
                        Data = entity,
                        Timestamp = DateTime.Now
                    };
                    request.SignIt(PasswordSha1);
                    ResponseBase response = Request<ResponseBase>("ControlCenter", "AddOrUpdateMinerGroup", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region RemoveMinerGroupAsync
            public void RemoveMinerGroupAsync(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    RemoveMinerGroupRequest request = new RemoveMinerGroupRequest() {
                        MessageId = messageId,
                        LoginName = LoginName,
                        Timestamp = DateTime.Now
                    };
                    request.SignIt(PasswordSha1);
                    ResponseBase response = Request<ResponseBase>("ControlCenter", "RemoveMinerGroup", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region AddOrUpdateMineWorkAsync
            public void AddOrUpdateMineWorkAsync(MineWorkData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    entity.ModifiedOn = DateTime.Now;
                    AddOrUpdateMineWorkRequest request = new AddOrUpdateMineWorkRequest {
                        MessageId = messageId,
                        LoginName = LoginName,
                        Timestamp = DateTime.Now,
                        Data = entity
                    };
                    request.SignIt(PasswordSha1);
                    ResponseBase response = Request<ResponseBase>("ControlCenter", "AddOrUpdateMineWork", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region RemoveMineWorkAsync
            public void RemoveMineWorkAsync(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    RemoveMineWorkRequest request = new RemoveMineWorkRequest {
                        MessageId = messageId,
                        LoginName = LoginName,
                        Timestamp = DateTime.Now,
                        MineWorkId = id
                    };
                    request.SignIt(PasswordSha1);
                    ResponseBase response = Request<ResponseBase>("ControlCenter", "RemoveMineWork", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region SetMinerProfilePropertyAsync
            public void SetMinerProfilePropertyAsync(Guid workId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    SetMinerProfilePropertyRequest request = new SetMinerProfilePropertyRequest() {
                        MessageId = messageId,
                        LoginName = LoginName,
                        PropertyName = propertyName,
                        Value = value,
                        Timestamp = DateTime.Now,
                        WorkId = workId
                    };
                    request.SignIt(PasswordSha1);
                    ResponseBase response = Request<ResponseBase>("ControlCenter", "SetMinerProfileProperty", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region SetCoinProfilePropertyAsync
            public void SetCoinProfilePropertyAsync(Guid workId, Guid coinId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    SetCoinProfilePropertyRequest request = new SetCoinProfilePropertyRequest {
                        MessageId = messageId,
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
                });
            }
            #endregion

            #region SetPoolProfilePropertyAsync
            public void SetPoolProfilePropertyAsync(Guid workId, Guid poolId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    SetPoolProfilePropertyRequest request = new SetPoolProfilePropertyRequest {
                        MessageId = messageId,
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
                });
            }
            #endregion

            #region SetCoinKernelProfilePropertyAsync
            public void SetCoinKernelProfilePropertyAsync(Guid workId, Guid coinKernelId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    SetCoinKernelProfilePropertyRequest request = new SetCoinKernelProfilePropertyRequest {
                        MessageId = messageId,
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
                });
            }
            #endregion

            #region GetWallets
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public GetWalletsResponse GetWallets() {
                WalletsRequest request = new WalletsRequest {
                    MessageId = Guid.NewGuid()
                };
                GetWalletsResponse response = Request<GetWalletsResponse>("ControlCenter", "Wallets", request);
                return response;
            }
            #endregion

            #region AddOrUpdateWalletAsync
            public void AddOrUpdateWalletAsync(WalletData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    entity.ModifiedOn = DateTime.Now;
                    AddOrUpdateWalletRequest request = new AddOrUpdateWalletRequest {
                        LoginName = LoginName,
                        MessageId = messageId,
                        Timestamp = DateTime.Now,
                        Data = entity
                    };
                    request.SignIt(PasswordSha1);
                    ResponseBase response = Request<ResponseBase>("ControlCenter", "AddOrUpdateWallet", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region RemoveWalletAsync
            public void RemoveWalletAsync(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    RemoveWalletRequest request = new RemoveWalletRequest {
                        MessageId = messageId,
                        LoginName = LoginName,
                        Timestamp = DateTime.Now,
                        WalletId = id
                    };
                    request.SignIt(PasswordSha1);
                    ResponseBase response = Request<ResponseBase>("ControlCenter", "RemoveWallet", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region GetCalcConfigs
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public GetCalcConfigsResponse GetCalcConfigs() {
                CalcConfigsRequest request = new CalcConfigsRequest {
                    MessageId = Guid.NewGuid()
                };
                GetCalcConfigsResponse response = Request<GetCalcConfigsResponse>("ControlCenter", "CalcConfigs", request);
                return response;
            }
            #endregion

            #region SaveCalcConfigsAsync
            public void SaveCalcConfigsAsync(List<CalcConfigData> configs, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    if (configs == null || configs.Count == 0) {
                        return;
                    }
                    Guid messageId = Guid.NewGuid();
                    SaveCalcConfigsRequest request = new SaveCalcConfigsRequest {
                        Data = configs,
                        LoginName = LoginName,
                        MessageId = messageId,
                        Timestamp = DateTime.Now
                    };
                    request.SignIt(PasswordSha1);
                    ResponseBase response = Request<ResponseBase>("ControlCenter", "SaveCalcConfigs", request);
                    callback?.Invoke(response);
                });
            }
            #endregion
        }
    }
}