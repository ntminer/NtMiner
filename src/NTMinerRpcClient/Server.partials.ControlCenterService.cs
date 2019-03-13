using NTMiner.Controllers;
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
                        SignatureRequest request = new SignatureRequest() {
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
            public DataResponse<List<UserData>> GetUsers(Guid messageId) {
                try {
                    SignatureRequest request = new SignatureRequest {
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<List<UserData>> response = Request<DataResponse<List<UserData>>>(s_controllerName, nameof(IControlCenterController.Users), request);
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
                        DataRequest<UserData> request = new DataRequest<UserData>() {
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
                        DataRequest<UserData> request = new DataRequest<UserData>() {
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
                        DataRequest<String> request = new DataRequest<String>() {
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
            public void LoadClientAsync(Guid clientId, Action<DataResponse<ClientData>, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        LoadClientRequest request = new LoadClientRequest {
                            LoginName = SingleUser.LoginName,
                            ClientId = clientId
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        DataResponse<ClientData> response = Request<DataResponse<ClientData>>(s_controllerName, nameof(IControlCenterController.LoadClient), request);
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

            #region AddClientAsync
            public void AddClientAsync(string clientIp, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        DataRequest<string> request = new DataRequest<string>() {
                            LoginName = SingleUser.LoginName,
                            Data = clientIp
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IControlCenterController.AddClient), request);
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
            public DataResponse<List<MinerGroupData>> GetMinerGroups() {
                try {
                    SignatureRequest request = new SignatureRequest {
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<List<MinerGroupData>> response = Request<DataResponse<List<MinerGroupData>>>(s_controllerName, nameof(IControlCenterController.MinerGroups), request);
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
                        DataRequest<MinerGroupData> request = new DataRequest<MinerGroupData> {
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
                        DataRequest<Guid> request = new DataRequest<Guid>() {
                            LoginName = SingleUser.LoginName,
                            Data = id
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
                DataRequest<MineWorkData> request = new DataRequest<MineWorkData> {
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
                        DataRequest<Guid> request = new DataRequest<Guid> {
                            LoginName = SingleUser.LoginName,
                            Data = id
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
                    DataResponse<List<MineWorkData>> response = Request<DataResponse<List<MineWorkData>>>(s_controllerName, nameof(IControlCenterController.MineWorks), request);
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
                    DataRequest<Guid> request = new DataRequest<Guid>() {
                        LoginName = SingleUser.LoginName,
                        Data = workId
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<MinerProfileData> response = Request<DataResponse<MinerProfileData>>(s_controllerName, nameof(IControlCenterController.MinerProfile), request);
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
                    WorkProfileRequest request = new WorkProfileRequest {
                        LoginName = SingleUser.LoginName,
                        WorkId = workId,
                        DataId = coinId
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<CoinProfileData> response = Request<DataResponse<CoinProfileData>>(s_controllerName, nameof(IControlCenterController.CoinProfile), request);
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
                    WorkProfileRequest request = new WorkProfileRequest {
                        LoginName = SingleUser.LoginName,
                        WorkId = workId,
                        DataId = poolId
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<PoolProfileData> response = Request<DataResponse<PoolProfileData>>(s_controllerName, nameof(IControlCenterController.PoolProfile), request);
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
                    WorkProfileRequest request = new WorkProfileRequest {
                        LoginName = SingleUser.LoginName,
                        WorkId = workId,
                        DataId = coinKernelId
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<CoinKernelProfileData> response = Request<DataResponse<CoinKernelProfileData>>(s_controllerName, nameof(IControlCenterController.CoinKernelProfile), request);
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
            public DataResponse<List<WalletData>> GetWallets() {
                try {
                    SignatureRequest request = new SignatureRequest {
                        LoginName = SingleUser.LoginName,
                        MessageId = Guid.NewGuid()
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<List<WalletData>> response = Request<DataResponse<List<WalletData>>>(s_controllerName, nameof(IControlCenterController.Wallets), request);
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
                        DataRequest<WalletData> request = new DataRequest<WalletData>() {
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
                        DataRequest<Guid> request = new DataRequest<Guid>() {
                            LoginName = SingleUser.LoginName,
                            Data = id
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
            public DataResponse<List<PoolData>> GetPools() {
                try {
                    SignatureRequest request = new SignatureRequest {
                        LoginName = SingleUser.LoginName,
                        MessageId = Guid.NewGuid()
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<List<PoolData>> response = Request<DataResponse<List<PoolData>>>(s_controllerName, nameof(IControlCenterController.Pools), request);
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
                        DataRequest<PoolData> request = new DataRequest<PoolData> {
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
                        DataRequest<Guid> request = new DataRequest<Guid>() {
                            LoginName = SingleUser.LoginName,
                            Data = id
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
            public DataResponse<List<CalcConfigData>> GetCalcConfigs() {
                try {
                    CalcConfigsRequest request = new CalcConfigsRequest {
                        MessageId = Guid.NewGuid()
                    };
                    DataResponse<List<CalcConfigData>> response = Request<DataResponse<List<CalcConfigData>>>(s_controllerName, nameof(IControlCenterController.CalcConfigs), request);
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
            public DataResponse<List<ColumnsShowData>> GetColumnsShows() {
                try {
                    SignatureRequest request = new SignatureRequest {
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<List<ColumnsShowData>> response = Request<DataResponse<List<ColumnsShowData>>>(s_controllerName, nameof(IControlCenterController.ColumnsShows), request);
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
                        DataRequest<ColumnsShowData> request = new DataRequest<ColumnsShowData>() {
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
                        DataRequest<Guid> request = new DataRequest<Guid>() {
                            LoginName = SingleUser.LoginName,
                            Data = id
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