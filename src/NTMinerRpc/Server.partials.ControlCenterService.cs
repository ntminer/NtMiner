using NTMiner;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public class ControlCenterServiceFace {
            public static readonly ControlCenterServiceFace Instance = new ControlCenterServiceFace();

            private ControlCenterServiceFace() {
            }

            private IControlCenterService CreateService() {
                return new EmptyControlCenterService();
            }

            #region LoginAsync
            public void LoginAsync(string loginName, string password, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            LoginControlCenterRequest request = new LoginControlCenterRequest {
                                MessageId = messageId,
                                LoginName = loginName,
                                Timestamp = DateTime.Now
                            };
                            request.SignIt(password);
                            ResponseBase response = service.LoginControlCenter(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region LoadClientsAsync
            public void LoadClientsAsync(List<Guid> clientIds, Action<LoadClientsResponse> callback) {
                Guid messageId = Guid.NewGuid();
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            LoadClientsRequest request = new LoadClientsRequest {
                                MessageId = messageId,
                                LoginName = LoginName,
                                ClientIds = clientIds,
                                Timestamp = DateTime.Now
                            };
                            request.SignIt(PasswordSha1);
                            LoadClientsResponse response = service.LoadClients(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError<LoadClientsResponse>(messageId, e.Message));
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
                    Guid messageId = Guid.NewGuid();
                    try {
                        using (var service = CreateService()) {
                            GetCoinSnapshotsRequest request = new GetCoinSnapshotsRequest {
                                MessageId = messageId,
                                LoginName = LoginName,
                                Limit = limit,
                                CoinCodes = coinCodes,
                                Timestamp = DateTime.Now
                            };
                            request.SignIt(PasswordSha1);
                            GetCoinSnapshotsResponse response = service.GetLatestSnapshots(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError<GetCoinSnapshotsResponse>(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region LoadClientAsync
            public void LoadClientAsync(Guid clientId, Action<LoadClientResponse> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        LoadClientRequest request = new LoadClientRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            ClientId = clientId,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            LoadClientResponse response = service.LoadClient(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError<LoadClientResponse>(messageId, e.Message));
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
                    Guid messageId = Guid.NewGuid();
                    try {
                        using (var service = CreateService()) {
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
                            QueryClientsResponse response = service.QueryClients(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError<QueryClientsResponse>(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region UpdateClientAsync
            public void UpdateClientAsync(Guid clientId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        UpdateClientRequest request = new UpdateClientRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            ClientId = clientId,
                            PropertyName = propertyName,
                            Value = value,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            ResponseBase response = service.UpdateClient(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
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
                    using (var service = CreateService()) {
                        return service.GetMinerGroups(messageId);
                    }
                }
                catch (Exception e) {
                    return ResponseBase.ClientError<GetMinerGroupsResponse>(messageId, e.Message);
                }
            }
            #endregion

            #region AddOrUpdateMinerGroupAsync
            public void AddOrUpdateMinerGroupAsync(MinerGroupData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        entity.ModifiedOn = DateTime.Now;
                        AddOrUpdateMinerGroupRequest request = new AddOrUpdateMinerGroupRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Data = entity,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            ResponseBase response = service.AddOrUpdateMinerGroup(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region RemoveMinerGroupAsync
            public void RemoveMinerGroupAsync(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        RemoveMinerGroupRequest request = new RemoveMinerGroupRequest() {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            ResponseBase response = service.RemoveMinerGroup(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region AddOrUpdateMineWorkAsync
            public void AddOrUpdateMineWorkAsync(MineWorkData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        entity.ModifiedOn = DateTime.Now;
                        AddOrUpdateMineWorkRequest request = new AddOrUpdateMineWorkRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now,
                            Data = entity
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            ResponseBase response = service.AddOrUpdateMineWork(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region RemoveMineWorkAsync
            public void RemoveMineWorkAsync(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        RemoveMineWorkRequest request = new RemoveMineWorkRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now,
                            MineWorkId = id
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            ResponseBase response = service.RemoveMineWork(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region SetMinerProfilePropertyAsync
            public void SetMinerProfilePropertyAsync(Guid workId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        SetMinerProfilePropertyRequest request = new SetMinerProfilePropertyRequest() {
                            MessageId = messageId,
                            LoginName = LoginName,
                            PropertyName = propertyName,
                            Value = value,
                            Timestamp = DateTime.Now,
                            WorkId = workId
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            ResponseBase response = service.SetMinerProfileProperty(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region SetCoinProfilePropertyAsync
            public void SetCoinProfilePropertyAsync(Guid workId, Guid coinId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
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
                        using (var service = CreateService()) {
                            ResponseBase response = service.SetCoinProfileProperty(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region SetPoolProfilePropertyAsync
            public void SetPoolProfilePropertyAsync(Guid workId, Guid poolId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
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
                        using (var service = CreateService()) {
                            ResponseBase response = service.SetPoolProfileProperty(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region SetCoinKernelProfilePropertyAsync
            public void SetCoinKernelProfilePropertyAsync(Guid workId, Guid coinKernelId, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
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
                        using (var service = CreateService()) {
                            ResponseBase response = service.SetCoinKernelProfileProperty(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
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
                Guid messageId = Guid.NewGuid();
                try {
                    using (var service = CreateService()) {
                        return service.GetWallets(messageId);
                    }
                }
                catch (Exception e) {
                    return ResponseBase.ClientError<GetWalletsResponse>(messageId, e.Message);
                }
            }
            #endregion

            #region AddOrUpdateWalletAsync
            public void AddOrUpdateWalletAsync(WalletData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        entity.ModifiedOn = DateTime.Now;
                        AddOrUpdateWalletRequest request = new AddOrUpdateWalletRequest {
                            LoginName = LoginName,
                            MessageId = messageId,
                            Timestamp = DateTime.Now,
                            Data = entity
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            ResponseBase response = service.AddOrUpdateWallet(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region RemoveWalletAsync
            public void RemoveWalletAsync(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        RemoveWalletRequest request = new RemoveWalletRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now,
                            WalletId = id
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            ResponseBase response = service.RemoveWallet(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
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
                Guid messageId = Guid.NewGuid();
                try {
                    using (var service = CreateService()) {
                        return service.GetCalcConfigs(messageId);
                    }
                }
                catch (Exception e) {
                    return ResponseBase.ClientError<GetCalcConfigsResponse>(messageId, e.Message);
                }
            }
            #endregion

            #region SaveCalcConfigsAsync
            public void SaveCalcConfigsAsync(List<CalcConfigData> configs) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
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
                        using (var service = CreateService()) {
                            service.SaveCalcConfigs(request);
                        }
                    }
                    catch (Exception e) {
                        
                    }
                });
            }
            #endregion
        }

        public class EmptyControlCenterService : IControlCenterService {
            public ResponseBase AddOrUpdateMinerGroup(AddOrUpdateMinerGroupRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase AddOrUpdateMineWork(AddOrUpdateMineWorkRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase AddOrUpdateWallet(AddOrUpdateWalletRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public void Dispose() {
                
            }

            public GetCalcConfigsResponse GetCalcConfigs(Guid messageId) {
                return null;
            }

            public GetCoinSnapshotsResponse GetLatestSnapshots(GetCoinSnapshotsRequest request) {
                return null;
            }

            public GetMinerGroupsResponse GetMinerGroups(Guid messageId) {
                return null;
            }

            public GetWalletsResponse GetWallets(Guid messageId) {
                return null;
            }

            public LoadClientResponse LoadClient(LoadClientRequest request) {
                return null;
            }

            public LoadClientsResponse LoadClients(LoadClientsRequest request) {
                return null;
            }

            public ResponseBase LoginControlCenter(LoginControlCenterRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public QueryClientsResponse QueryClients(QueryClientsRequest request) {
                return null;
            }

            public ResponseBase RemoveMinerGroup(RemoveMinerGroupRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase RemoveMineWork(RemoveMineWorkRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase RemoveWallet(RemoveWalletRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase SetClientMinerProfileProperty(SetClientMinerProfilePropertyRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase SetCoinKernelProfileProperty(SetCoinKernelProfilePropertyRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase SetCoinProfileProperty(SetCoinProfilePropertyRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase SetMinerProfileProperty(SetMinerProfilePropertyRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase SetPoolProfileProperty(SetPoolProfilePropertyRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase StartMine(StartMineRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase StopMine(StopMineRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }

            public ResponseBase UpdateClient(UpdateClientRequest request) {
                return ResponseBase.Ok(request.MessageId);
            }
        }
    }
}