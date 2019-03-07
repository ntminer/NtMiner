using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public class ControlCenterServiceFace {
            public static readonly ControlCenterServiceFace Instance = new ControlCenterServiceFace();
            private static readonly string s_controllerName = ControllerUtil.GetControllerName<IControlCenterController>();

            private ControlCenterServiceFace() {
            }

            #region LoginAsync
            public void LoginAsync(string loginName, string password, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        LoginControlCenterRequest request = new LoginControlCenterRequest {
                            LoginName = loginName
                        };
                        request.SignIt(password);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.LoginControlCenter), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region GetUsers
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="messageId"></param>
            /// <returns></returns>
            public GetUsersResponse GetUsers(Guid messageId) {
                try {
                    UsersRequest request = new UsersRequest {
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    GetUsersResponse response = Request<GetUsersResponse>(s_controllerName, nameof(IControlCenterController.Users), request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region AddUserAsync
            public void AddUserAsync(UserData userData, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        AddUserRequest request = new AddUserRequest() {
                            LoginName = SingleUser.LoginName,
                            Data = userData
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.AddUser), request);
                        callback?.Invoke(response, null);
                    }
                    catch(Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region UpdateUserAsync
            public void UpdateUserAsync(UserData userData, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        UpdateUserRequest request = new UpdateUserRequest() {
                            LoginName = SingleUser.LoginName,
                            Data = userData
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.UpdateUser), request);
                        callback?.Invoke(response, null);
                    }
                    catch(Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region RemoveUserAsync
            public void RemoveUserAsync(string loginName, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RemoveUserRequest request = new RemoveUserRequest() {
                            LoginName = SingleUser.LoginName,
                            Data = loginName
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.RemoveUser), request);
                        callback?.Invoke(response, null);
                    }
                    catch(Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region GetLatestSnapshotsAsync
            public void GetLatestSnapshotsAsync(
                int limit,
                List<string> coinCodes,
                Action<GetCoinSnapshotsResponse, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        GetCoinSnapshotsRequest request = new GetCoinSnapshotsRequest {
                            LoginName = SingleUser.LoginName,
                            Limit = limit
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        GetCoinSnapshotsResponse response = Request<GetCoinSnapshotsResponse>(s_controllerName, nameof(IControlCenterController.LatestSnapshots), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region LoadClientAsync
            public void LoadClientAsync(Guid clientId, bool isPull, Action<LoadClientResponse, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        LoadClientRequest request = new LoadClientRequest {
                            LoginName = SingleUser.LoginName,
                            ClientId = clientId,
                            IsPull = isPull
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        LoadClientResponse response = Request<LoadClientResponse>(s_controllerName, nameof(IControlCenterController.LoadClient), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region QueryClientsAsync
            public void QueryClientsAsync(
                int pageIndex,
                int pageSize,
                bool isPull,
                DateTime? timeLimit,
                Guid? groupId,
                Guid? workId,
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
                Action<QueryClientsResponse, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        var request = new QueryClientsRequest {
                            LoginName = SingleUser.LoginName,
                            PageIndex = pageIndex,
                            PageSize = pageSize,
                            IsPull = isPull,
                            TimeLimit = timeLimit,
                            GroupId = groupId,
                            WorkId = workId,
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
                            Kernel = kernel
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        QueryClientsResponse response = Request<QueryClientsResponse>(s_controllerName, nameof(IControlCenterController.QueryClients), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region UpdateClientAsync
            public void UpdateClientAsync(Guid clientId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        UpdateClientRequest request = new UpdateClientRequest {
                            LoginName = SingleUser.LoginName,
                            ClientId = clientId,
                            PropertyName = propertyName,
                            Value = value
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.UpdateClient), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region UpdateClientPropertiesAsync
            public void UpdateClientPropertiesAsync(Guid clientId, Dictionary<string, object> values, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        UpdateClientPropertiesRequest request = new UpdateClientPropertiesRequest {
                            LoginName = SingleUser.LoginName,
                            ClientId = clientId,
                            Values = values
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.UpdateClientProperties), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region GetMinerGroups
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public GetMinerGroupsResponse GetMinerGroups() {
                try {
                    MinerGroupsRequest request = new MinerGroupsRequest {
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    GetMinerGroupsResponse response = Request<GetMinerGroupsResponse>(s_controllerName, nameof(IControlCenterController.MinerGroups), request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region AddOrUpdateMinerGroupAsync
            public void AddOrUpdateMinerGroupAsync(MinerGroupData entity, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        entity.ModifiedOn = DateTime.Now;
                        AddOrUpdateMinerGroupRequest request = new AddOrUpdateMinerGroupRequest {
                            LoginName = SingleUser.LoginName,
                            Data = entity
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.AddOrUpdateMinerGroup), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region RemoveMinerGroupAsync
            public void RemoveMinerGroupAsync(Guid id, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RemoveMinerGroupRequest request = new RemoveMinerGroupRequest() {
                            LoginName = SingleUser.LoginName,
                            MinerGroupId = id
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.RemoveMinerGroup), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region AddOrUpdateMineWorkAsync
            public void AddOrUpdateMineWorkAsync(MineWorkData entity, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        entity.ModifiedOn = DateTime.Now;
                        AddOrUpdateMineWorkRequest request = new AddOrUpdateMineWorkRequest {
                            LoginName = SingleUser.LoginName,
                            Data = entity
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.AddOrUpdateMineWork), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region RemoveMineWorkAsync
            public void RemoveMineWorkAsync(Guid id, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RemoveMineWorkRequest request = new RemoveMineWorkRequest {
                            LoginName = SingleUser.LoginName,
                            MineWorkId = id
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.RemoveMineWork), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region SetMinerProfilePropertyAsync
            public void SetMinerProfilePropertyAsync(Guid workId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        SetMinerProfilePropertyRequest request = new SetMinerProfilePropertyRequest() {
                            LoginName = SingleUser.LoginName,
                            PropertyName = propertyName,
                            Value = value,
                            WorkId = workId
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.SetMinerProfileProperty), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region SetCoinProfilePropertyAsync
            public void SetCoinProfilePropertyAsync(Guid workId, Guid coinId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        SetCoinProfilePropertyRequest request = new SetCoinProfilePropertyRequest {
                            LoginName = SingleUser.LoginName,
                            CoinId = coinId,
                            WorkId = workId,
                            PropertyName = propertyName,
                            Value = value
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.SetCoinProfileProperty), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region SetPoolProfilePropertyAsync
            public void SetPoolProfilePropertyAsync(Guid workId, Guid poolId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        SetPoolProfilePropertyRequest request = new SetPoolProfilePropertyRequest {
                            LoginName = SingleUser.LoginName,
                            PoolId = poolId,
                            WorkId = workId,
                            PropertyName = propertyName,
                            Value = value
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.SetPoolProfileProperty), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region SetCoinKernelProfilePropertyAsync
            public void SetCoinKernelProfilePropertyAsync(Guid workId, Guid coinKernelId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        SetCoinKernelProfilePropertyRequest request = new SetCoinKernelProfilePropertyRequest {
                            LoginName = SingleUser.LoginName,
                            CoinKernelId = coinKernelId,
                            PropertyName = propertyName,
                            Value = value,
                            WorkId = workId
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.SetCoinKernelProfileProperty), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
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
                    GetWalletsResponse response = Request<GetWalletsResponse>(s_controllerName, nameof(IControlCenterController.Wallets), request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region AddOrUpdateWalletAsync
            public void AddOrUpdateWalletAsync(WalletData entity, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        entity.ModifiedOn = DateTime.Now;
                        AddOrUpdateWalletRequest request = new AddOrUpdateWalletRequest {
                            LoginName = SingleUser.LoginName,
                            Data = entity
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.AddOrUpdateWallet), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region RemoveWalletAsync
            public void RemoveWalletAsync(Guid id, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RemoveWalletRequest request = new RemoveWalletRequest {
                            LoginName = SingleUser.LoginName,
                            WalletId = id
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.RemoveWallet), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
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
                    GetCalcConfigsResponse response = Request<GetCalcConfigsResponse>(s_controllerName, nameof(IControlCenterController.CalcConfigs), request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region SaveCalcConfigsAsync
            public void SaveCalcConfigsAsync(List<CalcConfigData> configs, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        if (configs == null || configs.Count == 0) {
                            return;
                        }
                        SaveCalcConfigsRequest request = new SaveCalcConfigsRequest {
                            Data = configs,
                            LoginName = SingleUser.LoginName
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.SaveCalcConfigs), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region GetColumnsShows
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public GetColumnsShowsResponse GetColumnsShows() {
                try {
                    ColumnsShowsRequest request = new ColumnsShowsRequest {
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    GetColumnsShowsResponse response = Request<GetColumnsShowsResponse>(s_controllerName, nameof(IControlCenterController.ColumnsShows), request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region AddOrUpdateColumnsShowAsync
            public void AddOrUpdateColumnsShowAsync(ColumnsShowData entity, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        AddOrUpdateColumnsShowRequest request = new AddOrUpdateColumnsShowRequest {
                            LoginName = SingleUser.LoginName,
                            Data = entity
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.AddOrUpdateColumnsShow), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region RemoveColumnsShowAsync
            public void RemoveColumnsShowAsync(Guid id, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RemoveColumnsShowRequest request = new RemoveColumnsShowRequest() {
                            LoginName = SingleUser.LoginName,
                            ColumnsShowId = id
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.RemoveColumnsShow), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion
        }
    }
}