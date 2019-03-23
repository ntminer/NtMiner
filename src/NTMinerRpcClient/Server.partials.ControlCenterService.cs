using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;
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
                            Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{WebApiConst.ControlCenterPort}/api/{SControllerName}/{nameof(IControlCenterController.GetServicesVersion)}", null);
                            string response = message.Result.Content.ReadAsAsync<string>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        callback?.Invoke(string.Empty, e);
                    }
                });
            }

            public void CloseServices() {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{WebApiConst.ControlCenterPort}/api/{SControllerName}/{nameof(IControlCenterController.CloseServices)}", null);
                        Write.DevLine("CloseServices " + message.Result.ReasonPhrase);
                    }
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    Logger.ErrorDebugLine(e.Message, e);
                }
            }

            #region ActiveControlCenterAdminAsync
            public void ActiveControlCenterAdminAsync(string password, Action<ResponseBase, Exception> callback) {
                PostAsync(SControllerName, nameof(IControlCenterController.ActiveControlCenterAdmin), password, callback);
            }
            #endregion

            #region LoginAsync
            public void LoginAsync(string loginName, string password, Action<ResponseBase, Exception> callback) {
                SignatureRequest request = new SignatureRequest() {
                    LoginName = loginName
                };
                request.SignIt(password);
                PostAsync(SControllerName, nameof(IControlCenterController.LoginControlCenter), request, callback);
            }
            #endregion

            #region GetUsers
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="clientId"></param>
            /// <returns></returns>
            public List<UserData> GetUsers(Guid? clientId) {
                try {
                    DataRequest<Guid?> request = new DataRequest<Guid?> {
                        LoginName = SingleUser.LoginName,
                        Data = clientId
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<List<UserData>> response = Post<DataResponse<List<UserData>>>(SControllerName, nameof(IControlCenterController.Users), request);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<UserData>();
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    Logger.ErrorDebugLine(e.Message, e);
                    return new List<UserData>();
                }
            }
            #endregion

            #region AddUserAsync
            public void AddUserAsync(UserData userData, Action<ResponseBase, Exception> callback) {
                DataRequest<UserData> request = new DataRequest<UserData>() {
                    LoginName = SingleUser.LoginName,
                    Data = userData
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.AddUser), request, callback);
            }
            #endregion

            #region UpdateUserAsync
            public void UpdateUserAsync(UserData userData, Action<ResponseBase, Exception> callback) {
                DataRequest<UserData> request = new DataRequest<UserData>() {
                    LoginName = SingleUser.LoginName,
                    Data = userData
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.UpdateUser), request, callback);
            }
            #endregion

            #region RemoveUserAsync
            public void RemoveUserAsync(string loginName, Action<ResponseBase, Exception> callback) {
                DataRequest<String> request = new DataRequest<String>() {
                    LoginName = SingleUser.LoginName,
                    Data = loginName
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.RemoveUser), request, callback);
            }
            #endregion

            #region GetLatestSnapshotsAsync
            public void GetLatestSnapshotsAsync(
                int limit,
                List<string> coinCodes,
                Action<GetCoinSnapshotsResponse, Exception> callback) {
                GetCoinSnapshotsRequest request = new GetCoinSnapshotsRequest {
                    LoginName = SingleUser.LoginName,
                    Limit = limit
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.LatestSnapshots), request, callback);
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
                    LoginName = SingleUser.LoginName,
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
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.QueryClients), request, callback);
            }
            #endregion

            #region AddClientsAsync
            public void AddClientsAsync(List<string> clientIps, Action<ResponseBase, Exception> callback) {
                AddClientRequest request = new AddClientRequest() {
                    LoginName = SingleUser.LoginName,
                    ClientIps = clientIps
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.AddClients), request, callback);
            }
            #endregion

            #region RemoveClientsAsync
            public void RemoveClientsAsync(List<string> objectIds, Action<ResponseBase, Exception> callback) {
                MinerIdsRequest request = new MinerIdsRequest() {
                    LoginName = SingleUser.LoginName,
                    ObjectIds = objectIds
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.RemoveClients), request, callback);
            }
            #endregion

            #region RefreshClientsAsync
            public void RefreshClientsAsync(List<string> objectIds, Action<DataResponse<List<ClientData>>, Exception> callback) {
                MinerIdsRequest request = new MinerIdsRequest() {
                    LoginName = SingleUser.LoginName,
                    ObjectIds = objectIds
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.RefreshClients), request, callback);
            }
            #endregion

            #region UpdateClientAsync
            public void UpdateClientAsync(string objectId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                UpdateClientRequest request = new UpdateClientRequest {
                    LoginName = SingleUser.LoginName,
                    ObjectId = objectId,
                    PropertyName = propertyName,
                    Value = value
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.UpdateClient), request, callback);
            }
            #endregion

            #region UpdateClientsAsync
            public void UpdateClientsAsync(string propertyName, Dictionary<string, object> values, Action<ResponseBase, Exception> callback) {
                UpdateClientsRequest request = new UpdateClientsRequest {
                    LoginName = SingleUser.LoginName,
                    PropertyName = propertyName,
                    Values = values
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.UpdateClients), request, callback);
            }
            #endregion

            #region GetMinerGroups
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<MinerGroupData> GetMinerGroups() {
                try {
                    SignatureRequest request = new SignatureRequest {
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<List<MinerGroupData>> response = Post<DataResponse<List<MinerGroupData>>>(SControllerName, nameof(IControlCenterController.MinerGroups), request);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<MinerGroupData>();
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    Logger.ErrorDebugLine(e.Message, e);
                    return new List<MinerGroupData>();
                }
            }
            #endregion

            #region AddOrUpdateMinerGroupAsync
            public void AddOrUpdateMinerGroupAsync(MinerGroupData entity, Action<ResponseBase, Exception> callback) {
                entity.ModifiedOn = DateTime.Now;
                DataRequest<MinerGroupData> request = new DataRequest<MinerGroupData> {
                    LoginName = SingleUser.LoginName,
                    Data = entity
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.AddOrUpdateMinerGroup), request, callback);
            }
            #endregion

            #region RemoveMinerGroupAsync
            public void RemoveMinerGroupAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    LoginName = SingleUser.LoginName,
                    Data = id
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.RemoveMinerGroup), request, callback);
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
                        e = e.GetInnerException();
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
                ResponseBase response = Post<ResponseBase>(SControllerName, nameof(IControlCenterController.AddOrUpdateMineWork), request);
                return response;
            }
            #endregion

            #region RemoveMineWorkAsync
            public void RemoveMineWorkAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid> {
                    LoginName = SingleUser.LoginName,
                    Data = id
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.RemoveMineWork), request, callback);
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
                    DataResponse<List<MineWorkData>> response = Post<DataResponse<List<MineWorkData>>>(SControllerName, nameof(IControlCenterController.MineWorks), request);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<MineWorkData>();
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    Logger.ErrorDebugLine(e.Message, e);
                    return new List<MineWorkData>();
                }
            }
            #endregion

            #region ExportMineWorkAsync
            public void ExportMineWorkAsync(Guid workId, string localJson, string serverJson, Action<ResponseBase, Exception> callback) {
                ExportMineWorkRequest request = new ExportMineWorkRequest {
                    LoginName = SingleUser.LoginName,
                    MineWorkId = workId,
                    LocalJson = localJson,
                    ServerJson = serverJson
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.ExportMineWork), request, callback);
            }
            #endregion

            public string GetLocalJson(Guid workId) {
                try {
                    DataRequest<Guid> request = new DataRequest<Guid>() {
                        Data = workId,
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    var response = Post<DataResponse<string>>(SControllerName, nameof(IControlCenterController.GetLocalJson), request);
                    if (response != null) {
                        return response.Data;
                    }
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    Logger.ErrorDebugLine(e.Message, e);
                }
                return string.Empty;
            }

            #region GetWallets
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public DataResponse<List<WalletData>> GetWallets() {
                try {
                    SignatureRequest request = new SignatureRequest {
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<List<WalletData>> response = Post<DataResponse<List<WalletData>>>(SControllerName, nameof(IControlCenterController.Wallets), request);
                    return response;
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region AddOrUpdateWalletAsync
            public void AddOrUpdateWalletAsync(WalletData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<WalletData> request = new DataRequest<WalletData>() {
                    LoginName = SingleUser.LoginName,
                    Data = entity
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.AddOrUpdateWallet), request, callback);
            }
            #endregion

            #region RemoveWalletAsync
            public void RemoveWalletAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    LoginName = SingleUser.LoginName,
                    Data = id
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.RemoveWallet), request, callback);
            }
            #endregion

            #region GetPools
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<PoolData> GetPools() {
                try {
                    SignatureRequest request = new SignatureRequest {
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<List<PoolData>> response = Post<DataResponse<List<PoolData>>>(SControllerName, nameof(IControlCenterController.Pools), request);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<PoolData>();
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    Logger.ErrorDebugLine(e.Message, e);
                    return new List<PoolData>();
                }
            }
            #endregion

            #region AddOrUpdatePoolAsync
            public void AddOrUpdatePoolAsync(PoolData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<PoolData> request = new DataRequest<PoolData> {
                    LoginName = SingleUser.LoginName,
                    Data = entity
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.AddOrUpdatePool), request, callback);
            }
            #endregion

            #region RemovePoolAsync
            public void RemovePoolAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    LoginName = SingleUser.LoginName,
                    Data = id
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.RemovePool), request, callback);
            }
            #endregion

            #region GetColumnsShows
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<ColumnsShowData> GetColumnsShows() {
                try {
                    SignatureRequest request = new SignatureRequest {
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    DataResponse<List<ColumnsShowData>> response = Post<DataResponse<List<ColumnsShowData>>>(SControllerName, nameof(IControlCenterController.ColumnsShows), request);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<ColumnsShowData>();
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    Logger.ErrorDebugLine(e.Message, e);
                    return new List<ColumnsShowData>();
                }
            }
            #endregion

            #region AddOrUpdateColumnsShowAsync
            public void AddOrUpdateColumnsShowAsync(ColumnsShowData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<ColumnsShowData> request = new DataRequest<ColumnsShowData>() {
                    LoginName = SingleUser.LoginName,
                    Data = entity
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.AddOrUpdateColumnsShow), request, callback);
            }
            #endregion

            #region RemoveColumnsShowAsync
            public void RemoveColumnsShowAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    LoginName = SingleUser.LoginName,
                    Data = id
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IControlCenterController.RemoveColumnsShow), request, callback);
            }
            #endregion
        }
    }
}