using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public class ControlCenterServiceFace {
            public static readonly ControlCenterServiceFace Instance = new ControlCenterServiceFace();
            private readonly string baseUrl = $"http://{MinerServerHost}:{MinerServerPort}/api/ControlCenter";

            private ControlCenterServiceFace() {
            }

            #region LoginAsync
            public void LoginAsync(string loginName, string password, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        LoginControlCenterRequest request = new LoginControlCenterRequest {
                            MessageId = messageId,
                            LoginName = loginName,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(password);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/LoginControlCenter", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                }
            }
            #endregion

            #region LoadClientsAsync
            public void LoadClientsAsync(List<Guid> clientIds, Action<LoadClientsResponse> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        LoadClientsRequest request = new LoadClientsRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            ClientIds = clientIds,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/LoadClients", request);
                        LoadClientsResponse response = message.Result.Content.ReadAsAsync<LoadClientsResponse>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<LoadClientsResponse>(messageId, e.Message));
                }
            }
            #endregion

            #region GetLatestSnapshotsAsync
            public void GetLatestSnapshotsAsync(
                int limit,
                List<string> coinCodes,
                Action<GetCoinSnapshotsResponse> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        GetCoinSnapshotsRequest request = new GetCoinSnapshotsRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Limit = limit,
                            CoinCodes = coinCodes,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/LatestSnapshots", request);
                        GetCoinSnapshotsResponse response = message.Result.Content.ReadAsAsync<GetCoinSnapshotsResponse>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<GetCoinSnapshotsResponse>(messageId, e.Message));
                }
            }
            #endregion

            #region LoadClientAsync
            public void LoadClientAsync(Guid clientId, Action<LoadClientResponse> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        LoadClientRequest request = new LoadClientRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            ClientId = clientId,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/LoadClient", request);
                        LoadClientResponse response = message.Result.Content.ReadAsAsync<LoadClientResponse>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<LoadClientResponse>(messageId, e.Message));
                }
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
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
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
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/QueryClients", request);
                        QueryClientsResponse response = message.Result.Content.ReadAsAsync<QueryClientsResponse>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<QueryClientsResponse>(messageId, e.Message));
                }
            }
            #endregion

            #region UpdateClientAsync
            public void UpdateClientAsync(Guid clientId, string propertyName, object value, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        UpdateClientRequest request = new UpdateClientRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            ClientId = clientId,
                            PropertyName = propertyName,
                            Value = value,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/UpdateClient", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
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
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.GetAsync($"{baseUrl}/MinerGroups?messageId={messageId}");
                        GetMinerGroupsResponse response = message.Result.Content.ReadAsAsync<GetMinerGroupsResponse>().Result;
                        return response;
                    }
                }
                catch (Exception e) {
                    return ResponseBase.ClientError<GetMinerGroupsResponse>(messageId, e.Message);
                }
            }
            #endregion

            #region AddOrUpdateMinerGroupAsync
            public void AddOrUpdateMinerGroupAsync(MinerGroupData entity, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        entity.ModifiedOn = DateTime.Now;
                        AddOrUpdateMinerGroupRequest request = new AddOrUpdateMinerGroupRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Data = entity,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/AddOrUpdateMinerGroup", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion

            #region RemoveMinerGroupAsync
            public void RemoveMinerGroupAsync(Guid id, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        RemoveMinerGroupRequest request = new RemoveMinerGroupRequest() {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/RemoveMinerGroup", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion

            #region AddOrUpdateMineWorkAsync
            public void AddOrUpdateMineWorkAsync(MineWorkData entity, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        entity.ModifiedOn = DateTime.Now;
                        AddOrUpdateMineWorkRequest request = new AddOrUpdateMineWorkRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now,
                            Data = entity
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/AddOrUpdateMineWork", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion

            #region RemoveMineWorkAsync
            public void RemoveMineWorkAsync(Guid id, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        RemoveMineWorkRequest request = new RemoveMineWorkRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now,
                            MineWorkId = id
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/RemoveMineWork", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion

            #region SetMinerProfilePropertyAsync
            public void SetMinerProfilePropertyAsync(Guid workId, string propertyName, object value, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        SetMinerProfilePropertyRequest request = new SetMinerProfilePropertyRequest() {
                            MessageId = messageId,
                            LoginName = LoginName,
                            PropertyName = propertyName,
                            Value = value,
                            Timestamp = DateTime.Now,
                            WorkId = workId
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/SetMinerProfileProperty", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion

            #region SetCoinProfilePropertyAsync
            public void SetCoinProfilePropertyAsync(Guid workId, Guid coinId, string propertyName, object value, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
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
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/SetCoinProfileProperty", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion

            #region SetPoolProfilePropertyAsync
            public void SetPoolProfilePropertyAsync(Guid workId, Guid poolId, string propertyName, object value, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
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
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/SetPoolProfileProperty", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion

            #region SetCoinKernelProfilePropertyAsync
            public void SetCoinKernelProfilePropertyAsync(Guid workId, Guid coinKernelId, string propertyName, object value, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
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
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/SetCoinKernelProfileProperty", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion

            #region GetWallets
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public GetWalletsResponse GetWallets() {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.GetAsync($"{baseUrl}/Wallets?messageId={messageId}");
                        GetWalletsResponse response = message.Result.Content.ReadAsAsync<GetWalletsResponse>().Result;
                        return response;
                    }
                }
                catch (Exception e) {
                    return ResponseBase.ClientError<GetWalletsResponse>(messageId, e.Message);
                }
            }
            #endregion

            #region AddOrUpdateWalletAsync
            public void AddOrUpdateWalletAsync(WalletData entity, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        entity.ModifiedOn = DateTime.Now;
                        AddOrUpdateWalletRequest request = new AddOrUpdateWalletRequest {
                            LoginName = LoginName,
                            MessageId = messageId,
                            Timestamp = DateTime.Now,
                            Data = entity
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/AddOrUpdateWallet", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion

            #region RemoveWalletAsync
            public void RemoveWalletAsync(Guid id, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        RemoveWalletRequest request = new RemoveWalletRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now,
                            WalletId = id
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/RemoveWallet", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion

            #region GetCalcConfigs
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public GetCalcConfigsResponse GetCalcConfigs() {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.GetAsync($"{baseUrl}/CalcConfigs?messageId={messageId}");
                        GetCalcConfigsResponse response = message.Result.Content.ReadAsAsync<GetCalcConfigsResponse>().Result;
                        return response;
                    }
                }
                catch (Exception e) {
                    return ResponseBase.ClientError<GetCalcConfigsResponse>(messageId, e.Message);
                }
            }
            #endregion

            #region SaveCalcConfigsAsync
            public void SaveCalcConfigsAsync(List<CalcConfigData> configs, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        if (configs == null || configs.Count == 0) {
                            return;
                        }
                        SaveCalcConfigsRequest request = new SaveCalcConfigsRequest {
                            Data = configs,
                            LoginName = LoginName,
                            MessageId = messageId,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/SaveCalcConfigs", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion
        }
    }
}