using NTMiner.Core;
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

            #region ActiveControlCenterAdminAsync
            public void ActiveControlCenterAdminAsync(string password, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.ActiveControlCenterAdmin), password);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

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
                    SignatureRequest request = new SignatureRequest {
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
                    catch (Exception e) {
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
                    catch (Exception e) {
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
                    catch (Exception e) {
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
                    SignatureRequest request = new SignatureRequest {
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
                        var response = AddOrUpdateMineWork(entity);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region AddOrUpdateMineWork
            public ResponseBase AddOrUpdateMineWork(MineWorkData entity) {
                entity.ModifiedOn = DateTime.Now;
                AddOrUpdateMineWorkRequest request = new AddOrUpdateMineWorkRequest {
                    LoginName = SingleUser.LoginName,
                    Data = entity
                };
                request.SignIt(SingleUser.PasswordSha1);
                ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.AddOrUpdateMineWork), request);
                return response;
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

            #region GetMineWorks
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<MineWorkData> GetMineWorks() {
                try {
                    SignatureRequest request = new SignatureRequest {
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    GetMineWorksResponse response = Request<GetMineWorksResponse>(s_controllerName, nameof(IControlCenterController.MineWorks), request);
                    if (response != null) {
                        return response.Data;
                    }
                    return new List<MineWorkData>();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return new List<MineWorkData>();
                }
            }
            #endregion

            #region ExportMineWorkAsync
            public void ExportMineWorkAsync(Guid workId, string localJson, string serverJson, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        ExportMineWorkRequest request = new ExportMineWorkRequest {
                            LoginName = SingleUser.LoginName,
                            MineWorkId = workId,
                            LocalJson = localJson,
                            ServerJson = serverJson
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.ExportMineWork), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region GetMinerProfile
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <returns></returns>
            public MinerProfileData GetMinerProfile(Guid workId) {
                try {
                    MinerProfileRequest request = new MinerProfileRequest {
                        LoginName = SingleUser.LoginName,
                        WorkId = workId
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    MinerProfileResponse response = Request<MinerProfileResponse>(s_controllerName, nameof(IControlCenterController.MinerProfile), request);
                    if (response != null) {
                        return response.Data;
                    }
                    return null;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region GetCoinProfile
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <param name="coinId"></param>
            /// <returns></returns>
            public CoinProfileData GetCoinProfile(Guid workId, Guid coinId) {
                try {
                    CoinProfileRequest request = new CoinProfileRequest {
                        LoginName = SingleUser.LoginName,
                        WorkId = workId,
                        CoinId = coinId
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    CoinProfileResponse response = Request<CoinProfileResponse>(s_controllerName, nameof(IControlCenterController.CoinProfile), request);
                    if (response != null) {
                        return response.Data;
                    }
                    return null;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region GetPoolProfile
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <param name="poolId"></param>
            /// <returns></returns>
            public PoolProfileData GetPoolProfile(Guid workId, Guid poolId) {
                try {
                    PoolProfileRequest request = new PoolProfileRequest {
                        LoginName = SingleUser.LoginName,
                        WorkId = workId,
                        PoolId = poolId
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    PoolProfileResponse response = Request<PoolProfileResponse>(s_controllerName, nameof(IControlCenterController.PoolProfile), request);
                    if (response != null) {
                        return response.Data;
                    }
                    return null;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region GetCoinKernelProfile
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <param name="coinKernelId"></param>
            /// <returns></returns>
            public CoinKernelProfileData GetCoinKernelProfile(Guid workId, Guid coinKernelId) {
                try {
                    CoinKernelProfileRequest request = new CoinKernelProfileRequest {
                        LoginName = SingleUser.LoginName,
                        WorkId = workId,
                        CoinKernelId = coinKernelId
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    CoinKernelProfileResponse response = Request<CoinKernelProfileResponse>(s_controllerName, nameof(IControlCenterController.CoinKernelProfile), request);
                    if (response != null) {
                        return response.Data;
                    }
                    return null;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
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
                    SignatureRequest request = new SignatureRequest {
                        LoginName = SingleUser.LoginName,
                        MessageId = Guid.NewGuid()
                    };
                    request.SignIt(SingleUser.PasswordSha1);
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

            #region GetPools
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public GetPoolsResponse GetPools() {
                try {
                    SignatureRequest request = new SignatureRequest {
                        LoginName = SingleUser.LoginName,
                        MessageId = Guid.NewGuid()
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    GetPoolsResponse response = Request<GetPoolsResponse>(s_controllerName, nameof(IControlCenterController.Pools), request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region AddOrUpdatePoolAsync
            public void AddOrUpdatePoolAsync(PoolData entity, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        AddOrUpdatePoolRequest request = new AddOrUpdatePoolRequest {
                            LoginName = SingleUser.LoginName,
                            Data = entity
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.AddOrUpdatePool), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region RemovePoolAsync
            public void RemovePoolAsync(Guid id, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RemovePoolRequest request = new RemovePoolRequest {
                            LoginName = SingleUser.LoginName,
                            PoolId = id
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.RemovePool), request);
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
                    SignatureRequest request = new SignatureRequest {
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