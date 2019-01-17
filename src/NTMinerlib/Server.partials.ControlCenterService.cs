using NTMiner.ServiceContracts;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public class ControlCenterServiceFace {
            public static readonly ControlCenterServiceFace Instance = new ControlCenterServiceFace();

            private ControlCenterServiceFace() {
            }

            private IControlCenterService CreateService() {
                return ChannelFactory.CreateChannel<IControlCenterService>(MinerServerHost, MinerServerPort);
            }

            public void Login(string loginName, string password, Action<ResponseBase> callback) {
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
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }

            public void LoadClients(List<Guid> clientIds, Action<LoadClientsResponse> callback) {
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
                            request.SignIt(Password);
                            LoadClientsResponse response = service.LoadClients(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(LoadClientsResponse.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(LoadClientsResponse.ClientError(messageId, e.Message));
                    }
                });
            }

            public void GetLatestSnapshots(
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
                            request.SignIt(Password);
                            GetCoinSnapshotsResponse response = service.GetLatestSnapshots(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(GetCoinSnapshotsResponse.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(GetCoinSnapshotsResponse.ClientError(messageId, e.Message));
                    }
                });
            }

            public void LoadClient(Guid clientId, Action<LoadClientResponse> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        LoadClientRequest request = new LoadClientRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            ClientId = clientId,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            LoadClientResponse response = service.LoadClient(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(LoadClientResponse.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(LoadClientResponse.ClientError(messageId, e.Message));
                    }
                });
            }

            public void QueryClients(
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
                                Timestamp = DateTime.Now
                            };
                            request.SignIt(Password);
                            QueryClientsResponse response = service.QueryClients(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(QueryClientsResponse.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(QueryClientsResponse.ClientError(messageId, e.Message));
                    }
                });
            }

            public void UpdateClient(Guid clientId, string propertyName, object value, Action<ResponseBase> callback) {
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
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            ResponseBase response = service.UpdateClient(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }

            public GetMinerGroupsResponse GetMinerGroups(Guid messageId) {
                try {
                    using (var service = CreateService()) {
                        return service.GetMinerGroups(messageId);
                    }
                }
                catch (CommunicationException e) {
                    Global.DebugLine(e.Message, ConsoleColor.Red);
                    return GetMinerGroupsResponse.ClientError(messageId, e.Message);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    return GetMinerGroupsResponse.ClientError(messageId, e.Message);
                }
            }

            public void AddOrUpdateMinerGroup(MinerGroupData entity, Action<ResponseBase> callback) {
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
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            ResponseBase response = service.AddOrUpdateMinerGroup(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }

            public void RemoveMinerGroup(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        RemoveMinerGroupRequest request = new RemoveMinerGroupRequest() {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            ResponseBase response = service.RemoveMinerGroup(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }

            public void AddOrUpdateMineWork(MineWorkData entity, Action<ResponseBase> callback) {
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
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            ResponseBase response = service.AddOrUpdateMineWork(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }

            public void RemoveMineWork(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        RemoveMineWorkRequest request = new RemoveMineWorkRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now,
                            MineWorkId = id
                        };
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            ResponseBase response = service.RemoveMineWork(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }

            public void SetMinerProfileProperty(Guid workId, string propertyName, object value, Action<ResponseBase> callback) {
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
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            ResponseBase response = service.SetMinerProfileProperty(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }

            public void SetCoinProfileProperty(Guid workId, Guid coinId, string propertyName, object value, Action<ResponseBase> callback) {
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
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            ResponseBase response = service.SetCoinProfileProperty(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }

            public void SetPoolProfileProperty(Guid workId, Guid poolId, string propertyName, object value, Action<ResponseBase> callback) {
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
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            ResponseBase response = service.SetPoolProfileProperty(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }

            public void SetCoinKernelProfileProperty(Guid workId, Guid coinKernelId, string propertyName, object value, Action<ResponseBase> callback) {
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
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            ResponseBase response = service.SetCoinKernelProfileProperty(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }

            public GetWalletsResponse GetWallets() {
                Guid messageId = Guid.NewGuid();
                try {
                    using (var service = CreateService()) {
                        return service.GetWallets(messageId);
                    }
                }
                catch (CommunicationException e) {
                    Global.DebugLine(e.Message, ConsoleColor.Red);
                    return GetWalletsResponse.ClientError(messageId, e.Message);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    return GetWalletsResponse.ClientError(messageId, e.Message);
                }
            }

            public void AddOrUpdateWallet(WalletData entity, Action<ResponseBase> callback) {
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
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            ResponseBase response = service.AddOrUpdateWallet(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }

            public void RemoveWallet(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        RemoveWalletRequest request = new RemoveWalletRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now,
                            WalletId = id
                        };
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            ResponseBase response = service.RemoveWallet(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }

            public GetCalcConfigsResponse GetCalcConfigs() {
                Guid messageId = Guid.NewGuid();
                try {
                    using (var service = CreateService()) {
                        return service.GetCalcConfigs(messageId);
                    }
                }
                catch (CommunicationException e) {
                    Global.DebugLine(e.Message, ConsoleColor.Red);
                    return GetCalcConfigsResponse.ClientError(messageId, e.Message);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    return GetCalcConfigsResponse.ClientError(messageId, e.Message);
                }
            }

            public void SaveCalcConfigs(List<CalcConfigData> configs) {
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
                        request.SignIt(Password);
                        using (var service = CreateService()) {
                            service.SaveCalcConfigs(request);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }
        }
    }
}