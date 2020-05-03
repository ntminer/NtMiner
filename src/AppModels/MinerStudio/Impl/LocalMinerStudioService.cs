using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.Daemon;
using NTMiner.Core.Impl;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.Core.MinerStudio;
using NTMiner.JsonDb;
using NTMiner.VirtualMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NTMiner.MinerStudio.Impl {
    public class LocalMinerStudioService : ILocalMinerStudioService {
        private readonly string _daemonControllerName = RpcRoot.GetControllerName<INTMinerDaemonController>();

        private readonly IClientDataSet _clientDataSet;
        private readonly ICoinSnapshotSet _coinSnapshotSet;

        public LocalMinerStudioService() {
            _clientDataSet = new ClientDataSet();
            _coinSnapshotSet = new CoinSnapshotSet(_clientDataSet);
        }

        #region AddClientsAsync
        public void AddClientsAsync(List<string> clientIps, Action<ResponseBase, Exception> callback) {
            try {
                foreach (var clientIp in clientIps) {
                    ClientData clientData = _clientDataSet.AsEnumerable().FirstOrDefault(a => a.MinerIp == clientIp);
                    if (clientData != null) {
                        continue;
                    }
                    _clientDataSet.AddClient(clientIp);
                }
                callback?.Invoke(ResponseBase.Ok(), null);
            }
            catch (Exception e) {
                callback?.Invoke(ResponseBase.ServerError(e.Message), e);
            }
        }
        #endregion

        #region RemoveClientsAsync
        public void RemoveClientsAsync(List<string> objectIds, Action<ResponseBase, Exception> callback) {
            try {
                foreach (var objectId in objectIds) {
                    _clientDataSet.RemoveByObjectId(objectId);
                }
                callback?.Invoke(ResponseBase.Ok(), null);
            }
            catch (Exception e) {
                callback?.Invoke(ResponseBase.ServerError(e.Message), e);
            }
        }
        #endregion

        #region QueryClientsAsync
        public void QueryClientsAsync(QueryClientsRequest query, Action<QueryClientsResponse, Exception> callback) {
            try {
                var data = _clientDataSet.QueryClients(
                    user: null,
                    query,
                    out int total,
                    out List<CoinSnapshotData> latestSnapshots,
                    out int totalOnlineCount,
                    out int totalMiningCount);
                callback?.Invoke(QueryClientsResponse.Ok(data, total, latestSnapshots, totalMiningCount, totalOnlineCount), null);
            }
            catch (Exception e) {
                callback?.Invoke(ResponseBase.ServerError<QueryClientsResponse>(e.Message), e);
            }
        }
        #endregion

        #region UpdateClientAsync
        public void UpdateClientAsync(string objectId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
            try {
                _clientDataSet.UpdateClient(objectId, propertyName, value);
                callback?.Invoke(ResponseBase.Ok(), null);
            }
            catch (Exception e) {
                callback?.Invoke(ResponseBase.ServerError(e.Message), e);
            }
        }
        #endregion

        #region UpdateClientsAsync
        public void UpdateClientsAsync(string propertyName, Dictionary<string, object> values, Action<ResponseBase, Exception> callback) {
            try {
                _clientDataSet.UpdateClients(propertyName, values);
                callback?.Invoke(ResponseBase.Ok(), null);
            }
            catch (Exception e) {
                callback?.Invoke(ResponseBase.ServerError(e.Message), e);
            }
        }
        #endregion

        #region GetLatestSnapshotsAsync
        public void GetLatestSnapshotsAsync(int limit, Action<GetCoinSnapshotsResponse, Exception> callback) {
            try {
                List<CoinSnapshotData> data = _coinSnapshotSet.GetLatestSnapshots(
                    limit,
                    out int totalMiningCount,
                    out int totalOnlineCount) ?? new List<CoinSnapshotData>();
                callback?.Invoke(GetCoinSnapshotsResponse.Ok(data, totalMiningCount, totalOnlineCount), null);
            }
            catch (Exception e) {
                callback?.Invoke(ResponseBase.ServerError<GetCoinSnapshotsResponse>(e.Message), e);
            }
        }
        #endregion

        #region EnableRemoteDesktopAsync
        public void EnableRemoteDesktopAsync(IMinerData client) {
            JsonRpcRoot.PostAsync<ResponseBase>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.EnableRemoteDesktop), null, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region GetConsoleOutLinesAsync
        public void GetConsoleOutLinesAsync(IMinerData client, long afterTime) {
            RpcRoot.Client.MinerClientService.GetConsoleOutLinesAsync(client.GetLocalIp(), afterTime, (data, e) => {
                if (data != null && data.Count > 0) {
                    VirtualRoot.RaiseEvent(new ClientConsoleOutLinesEvent(client.ClientId, data));
                }
            });
        }
        #endregion

        #region GetLocalMessagesAsync
        public void GetLocalMessagesAsync(IMinerData client, long afterTime) {
            RpcRoot.Client.MinerClientService.GetLocalMessagesAsync(client.GetLocalIp(), afterTime, (data, e) => {
                if (data != null && data.Count > 0) {
                    VirtualRoot.RaiseEvent(new ClientLocalMessagesEvent(client.ClientId, data));
                }
            });
        }
        #endregion

        #region BlockWAUAsync
        public void BlockWAUAsync(IMinerData client) {
            JsonRpcRoot.PostAsync<ResponseBase>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.BlockWAU), null, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region AtikmdagPatcherAsync
        public void AtikmdagPatcherAsync(IMinerData client) {
            JsonRpcRoot.PostAsync<ResponseBase>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.AtikmdagPatcher), null, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region SwitchRadeonGpuAsync
        public void SwitchRadeonGpuAsync(IMinerData client, bool on) {
            JsonRpcRoot.PostAsync<ResponseBase>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.SwitchRadeonGpu), new Dictionary<string, string> {
                {"on", on.ToString() }
            }, null, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region RestartWindowsAsync
        public void RestartWindowsAsync(IMinerData client) {
            JsonRpcRoot.PostAsync<ResponseBase>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.RestartWindows), new object(), null, timeountMilliseconds: 3000);
        }
        #endregion

        #region ShutdownWindowsAsync
        public void ShutdownWindowsAsync(IMinerData client) {
            JsonRpcRoot.PostAsync<ResponseBase>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.ShutdownWindows), new object(), null, timeountMilliseconds: 3000);
        }
        #endregion

        #region UpgradeNTMinerAsync
        // ReSharper disable once InconsistentNaming
        public void UpgradeNTMinerAsync(IMinerData client, string ntminerFileName) {
            UpgradeNTMinerRequest request = new UpgradeNTMinerRequest {
                NTMinerFileName = ntminerFileName
            };
            JsonRpcRoot.PostAsync<ResponseBase>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.UpgradeNTMiner), request, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region SetAutoBootStartAsync
        public void SetAutoBootStartAsync(IMinerData client, SetAutoBootStartRequest request) {
            HttpRpcRoot.FirePostAsync(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.SetAutoBootStart), new Dictionary<string, string> {
                {"autoBoot", request.AutoBoot.ToString() },
                {"autoStart", request.AutoStart.ToString() }
            }, content: null, callback: null, timeountMilliseconds: 3000);
        }
        #endregion

        #region StartMineAsync
        public void StartMineAsync(IMinerData client, Guid workId) {
            string localJson = string.Empty, serverJson = string.Empty;
            if (workId != Guid.Empty) {
                localJson = MinerStudioPath.ReadMineWorkLocalJsonFile(workId).Replace(NTKeyword.MinerNameParameterName, client.WorkerName);
                serverJson = MinerStudioPath.ReadMineWorkServerJsonFile(workId);
            }
            WorkRequest request = new WorkRequest {
                WorkId = workId,
                WorkerName = client.WorkerName,
                LocalJson = localJson,
                ServerJson = serverJson
            };
            JsonRpcRoot.PostAsync<ResponseBase>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.StartMine), request, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region StopMineAsync
        public void StopMineAsync(IMinerData client) {
            JsonRpcRoot.PostAsync<ResponseBase>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.StopMine), new object(), null, timeountMilliseconds: 3000);
        }
        #endregion

        #region GetDrivesAsync
        public void GetDrivesAsync(IMinerData client) {
            JsonRpcRoot.PostAsync<List<DriveDto>>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.GetDrives), null, (data, e) => {
                VirtualRoot.RaiseEvent(new GetDrivesResponsedEvent(client.ClientId, data));
            }, timeountMilliseconds: 3000);
        }
        #endregion

        #region SetVirtualMemoryAsync
        public void SetVirtualMemoryAsync(IMinerData client, Dictionary<string, int> data) {
            JsonRpcRoot.PostAsync<ResponseBase>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.SetVirtualMemory), new DataRequest<Dictionary<string, int>> {
                Data = data
            }, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region GetLocalIpsAsync
        public void GetLocalIpsAsync(IMinerData client) {
            JsonRpcRoot.PostAsync<List<LocalIpDto>>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.GetLocalIps), null, (data, e) => {
                VirtualRoot.RaiseEvent(new GetLocalIpsResponsedEvent(client.ClientId, data));
            }, timeountMilliseconds: 3000);
        }
        #endregion

        #region SetLocalIpsAsync
        public void SetLocalIpsAsync(IMinerData client, List<LocalIpInput> data) {
            JsonRpcRoot.PostAsync<ResponseBase>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.SetLocalIps), new DataRequest<List<LocalIpInput>> {
                Data = data
            }, null, timeountMilliseconds: 3000);
        }
        #endregion

        #region GetOperationResultsAsync
        public void GetOperationResultsAsync(IMinerData client, long afterTime) {
            JsonRpcRoot.GetAsync<List<OperationResultData>>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.GetOperationResults), new Dictionary<string, string> {
                {"afterTime",afterTime.ToString() }
            }, (data, e) => {
                if (data != null && data.Count > 0) {
                    VirtualRoot.RaiseEvent(new ClientOperationResultsEvent(client.ClientId, data));
                }
            }, timeountMilliseconds: 3000);
        }
        #endregion

        #region GetSelfWorkLocalJsonAsync
        public void GetSelfWorkLocalJsonAsync(IMinerData client) {
            JsonRpcRoot.PostAsync<string>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.GetSelfWorkLocalJson), null, (json, e) => {
                VirtualRoot.RaiseEvent(new GetSelfWorkLocalJsonResponsedEvent(client.ClientId, json));
            }, timeountMilliseconds: 3000);
        }
        #endregion

        #region SaveSelfWorkLocalJsonAsync
        public void SaveSelfWorkLocalJsonAsync(IMinerData client, string localJson, string serverJson) {
            if (string.IsNullOrEmpty(localJson) || string.IsNullOrEmpty(serverJson)) {
                return;
            }
            WorkRequest request = new WorkRequest {
                WorkId = MineWorkData.SelfMineWorkId,
                WorkerName = client.WorkerName,
                LocalJson = localJson.Replace(NTKeyword.MinerNameParameterName, client.WorkerName),
                ServerJson = serverJson
            };
            JsonRpcRoot.FirePostAsync(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.SaveSelfWorkLocalJson), null, request, timeountMilliseconds: 3000);
        }
        #endregion

        #region GetGpuProfilesJsonAsync
        public void GetGpuProfilesJsonAsync(IMinerData client) {
            JsonRpcRoot.PostAsync<string>(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.GetGpuProfilesJson), null, (json, e) => {
                GpuProfilesJsonDb data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJsonDb>(json) ?? new GpuProfilesJsonDb();
                VirtualRoot.RaiseEvent(new GetGpuProfilesResponsedEvent(client.ClientId, data));
            }, timeountMilliseconds: 3000);
        }
        #endregion

        #region SaveGpuProfilesJsonAsync
        public void SaveGpuProfilesJsonAsync(IMinerData client, string json) {
            HttpContent content = new StringContent(json);
            HttpRpcRoot.FirePostAsync(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _daemonControllerName, nameof(INTMinerDaemonController.SaveGpuProfilesJson), null, content, null, timeountMilliseconds: 3000);
        }
        #endregion
    }
}
