using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public class ControlCenterServiceFace {
            public static readonly ControlCenterServiceFace Instance = new ControlCenterServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IControlCenterController>();

            private ControlCenterServiceFace() {
            }

            public void GetServicesVersionAsync(Action<string, Exception> callback) {
                Process[] processes = Process.GetProcessesByName("NTMinerServices");
                if (processes.Length == 0) {
                    callback?.Invoke(string.Empty, null);
                }
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{Consts.ControlCenterPort}/api/{SControllerName}/{nameof(IControlCenterController.GetServicesVersion)}", null);
                            string response = message.Result.Content.ReadAsAsync<string>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(string.Empty, e);
                    }
                });
            }

            public void CloseServices() {
                try {
                    Process[] processes = Process.GetProcessesByName("NTMinerServices");
                    if (processes.Length == 0) {
                        return;
                    }
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{Consts.ControlCenterPort}/api/{SControllerName}/{nameof(IControlCenterController.CloseServices)}", null);
                        Write.DevDebug($"{nameof(CloseServices)} {message.Result.ReasonPhrase}");
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
            }

            #region ActiveControlCenterAdminAsync
            public void ActiveControlCenterAdminAsync(string password, Action<ResponseBase, Exception> callback) {
                PostAsync(SControllerName, nameof(IControlCenterController.ActiveControlCenterAdmin), null, password, callback);
            }
            #endregion

            #region LoginAsync
            public void LoginAsync(string loginName, string password, Action<ResponseBase, Exception> callback) {
                SignRequest request = new SignRequest() {
                };
                PostAsync(SControllerName, nameof(IControlCenterController.LoginControlCenter), request.ToQuery(loginName, password), request, callback);
            }
            #endregion

            #region GetUsers
            // TODO:异步化
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="clientId"></param>
            /// <returns></returns>
            public List<UserData> GetUsers(Guid? clientId) {
                try {
                    DataRequest<Guid?> request = new DataRequest<Guid?> {
                        Data = clientId
                    };
                    DataResponse<List<UserData>> response = Post<DataResponse<List<UserData>>>(SControllerName, nameof(IControlCenterController.Users), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, timeout: 2000);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<UserData>();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return new List<UserData>();
                }
            }
            #endregion

            #region AddUserAsync
            public void AddUserAsync(UserData userData, Action<ResponseBase, Exception> callback) {
                DataRequest<UserData> request = new DataRequest<UserData>() {
                    Data = userData
                };
                PostAsync(SControllerName, nameof(IControlCenterController.AddUser), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region UpdateUserAsync
            public void UpdateUserAsync(UserData userData, Action<ResponseBase, Exception> callback) {
                DataRequest<UserData> request = new DataRequest<UserData>() {
                    Data = userData
                };
                PostAsync(SControllerName, nameof(IControlCenterController.UpdateUser), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region RemoveUserAsync
            public void RemoveUserAsync(string loginName, Action<ResponseBase, Exception> callback) {
                DataRequest<String> request = new DataRequest<String>() {
                    Data = loginName
                };
                PostAsync(SControllerName, nameof(IControlCenterController.RemoveUser), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region GetLatestSnapshotsAsync
            public void GetLatestSnapshotsAsync(
                int limit,
                Action<GetCoinSnapshotsResponse, Exception> callback) {
                GetCoinSnapshotsRequest request = new GetCoinSnapshotsRequest {
                    Limit = limit
                };
                PostAsync(SControllerName, nameof(IControlCenterController.LatestSnapshots), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
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
                PostAsync(SControllerName, nameof(IControlCenterController.QueryClients), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region AddClientsAsync
            public void AddClientsAsync(List<string> clientIps, Action<ResponseBase, Exception> callback) {
                AddClientRequest request = new AddClientRequest() {
                    ClientIps = clientIps
                };
                PostAsync(SControllerName, nameof(IControlCenterController.AddClients), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region RemoveClientsAsync
            public void RemoveClientsAsync(List<string> objectIds, Action<ResponseBase, Exception> callback) {
                MinerIdsRequest request = new MinerIdsRequest() {
                    ObjectIds = objectIds
                };
                PostAsync(SControllerName, nameof(IControlCenterController.RemoveClients), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region RefreshClientsAsync
            public void RefreshClientsAsync(List<string> objectIds, Action<DataResponse<List<ClientData>>, Exception> callback) {
                MinerIdsRequest request = new MinerIdsRequest() {
                    ObjectIds = objectIds
                };
                PostAsync(SControllerName, nameof(IControlCenterController.RefreshClients), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region UpdateClientAsync
            public void UpdateClientAsync(string objectId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                UpdateClientRequest request = new UpdateClientRequest {
                    ObjectId = objectId,
                    PropertyName = propertyName,
                    Value = value
                };
                PostAsync(SControllerName, nameof(IControlCenterController.UpdateClient), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region UpdateClientsAsync
            public void UpdateClientsAsync(string propertyName, Dictionary<string, object> values, Action<ResponseBase, Exception> callback) {
                UpdateClientsRequest request = new UpdateClientsRequest {
                    PropertyName = propertyName,
                    Values = values
                };
                PostAsync(SControllerName, nameof(IControlCenterController.UpdateClients), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region GetMinerGroups
            // TODO:异步化
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<MinerGroupData> GetMinerGroups() {
                try {
                    SignRequest request = new SignRequest {
                    };
                    DataResponse<List<MinerGroupData>> response = Post<DataResponse<List<MinerGroupData>>>(SControllerName, nameof(IControlCenterController.MinerGroups), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, timeout: 2000);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<MinerGroupData>();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return new List<MinerGroupData>();
                }
            }
            #endregion

            #region AddOrUpdateMinerGroupAsync
            public void AddOrUpdateMinerGroupAsync(MinerGroupData entity, Action<ResponseBase, Exception> callback) {
                entity.ModifiedOn = DateTime.Now;
                DataRequest<MinerGroupData> request = new DataRequest<MinerGroupData> {
                    Data = entity
                };
                PostAsync(SControllerName, nameof(IControlCenterController.AddOrUpdateMinerGroup), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region RemoveMinerGroupAsync
            public void RemoveMinerGroupAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                PostAsync(SControllerName, nameof(IControlCenterController.RemoveMinerGroup), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
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
                    Data = entity
                };
                ResponseBase response = Post<ResponseBase>(SControllerName, nameof(IControlCenterController.AddOrUpdateMineWork), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request);
                return response;
            }
            #endregion

            #region RemoveMineWorkAsync
            public void RemoveMineWorkAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid> {
                    Data = id
                };
                PostAsync(SControllerName, nameof(IControlCenterController.RemoveMineWork), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region GetMineWorks
            // TODO:异步化
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<MineWorkData> GetMineWorks() {
                try {
                    SignRequest request = new SignRequest {
                    };
                    DataResponse<List<MineWorkData>> response = Post<DataResponse<List<MineWorkData>>>(SControllerName, nameof(IControlCenterController.MineWorks), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, timeout: 2000);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<MineWorkData>();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return new List<MineWorkData>();
                }
            }
            #endregion

            #region ExportMineWorkAsync
            public void ExportMineWorkAsync(Guid workId, string localJson, string serverJson, Action<ResponseBase, Exception> callback) {
                ExportMineWorkRequest request = new ExportMineWorkRequest {
                    MineWorkId = workId,
                    LocalJson = localJson,
                    ServerJson = serverJson
                };
                PostAsync(SControllerName, nameof(IControlCenterController.ExportMineWork), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            public string GetLocalJson(Guid workId) {
                try {
                    DataRequest<Guid> request = new DataRequest<Guid>() {
                        Data = workId
                    };
                    var response = Post<DataResponse<string>>(SControllerName, nameof(IControlCenterController.GetLocalJson), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request);
                    if (response != null) {
                        return response.Data;
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                return string.Empty;
            }

            #region GetWallets
            // TODO:异步化
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public DataResponse<List<WalletData>> GetWallets() {
                try {
                    SignRequest request = new SignRequest {
                    };
                    DataResponse<List<WalletData>> response = Post<DataResponse<List<WalletData>>>(SControllerName, nameof(IControlCenterController.Wallets), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, timeout: 2000);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return null;
                }
            }
            #endregion

            #region AddOrUpdateWalletAsync
            public void AddOrUpdateWalletAsync(WalletData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<WalletData> request = new DataRequest<WalletData>() {
                    Data = entity
                };
                PostAsync(SControllerName, nameof(IControlCenterController.AddOrUpdateWallet), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region RemoveWalletAsync
            public void RemoveWalletAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                PostAsync(SControllerName, nameof(IControlCenterController.RemoveWallet), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region GetPools
            // TODO:异步化
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<PoolData> GetPools() {
                try {
                    SignRequest request = new SignRequest {
                    };
                    DataResponse<List<PoolData>> response = Post<DataResponse<List<PoolData>>>(SControllerName, nameof(IControlCenterController.Pools), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, timeout: 2000);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<PoolData>();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return new List<PoolData>();
                }
            }
            #endregion

            #region AddOrUpdatePoolAsync
            public void AddOrUpdatePoolAsync(PoolData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<PoolData> request = new DataRequest<PoolData> {
                    Data = entity
                };
                PostAsync(SControllerName, nameof(IControlCenterController.AddOrUpdatePool), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region RemovePoolAsync
            public void RemovePoolAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                PostAsync(SControllerName, nameof(IControlCenterController.RemovePool), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region GetColumnsShows
            // TODO:异步化
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<ColumnsShowData> GetColumnsShows() {
                try {
                    SignRequest request = new SignRequest {
                    };
                    DataResponse<List<ColumnsShowData>> response = Post<DataResponse<List<ColumnsShowData>>>(SControllerName, nameof(IControlCenterController.ColumnsShows), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, timeout: 2000);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<ColumnsShowData>();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return new List<ColumnsShowData>();
                }
            }
            #endregion

            #region AddOrUpdateColumnsShowAsync
            public void AddOrUpdateColumnsShowAsync(ColumnsShowData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<ColumnsShowData> request = new DataRequest<ColumnsShowData>() {
                    Data = entity
                };
                PostAsync(SControllerName, nameof(IControlCenterController.AddOrUpdateColumnsShow), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region RemoveColumnsShowAsync
            public void RemoveColumnsShowAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                PostAsync(SControllerName, nameof(IControlCenterController.RemoveColumnsShow), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion
        }
    }
}