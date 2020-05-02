using NTMiner.Core;
using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner.MinerStudio.Impl {
    public class ServerMinerStudioService : IServerMinerStudioService {
        public ServerMinerStudioService() {
        }

        #region QueryClientsAsync
        public void QueryClientsAsync(QueryClientsRequest query, Action<QueryClientsResponse, Exception> callback) {
            RpcRoot.OfficialServer.ClientDataService.QueryClientsAsync(query, callback);
        }
        #endregion

        #region UpdateClientAsync
        public void UpdateClientAsync(string objectId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
            RpcRoot.OfficialServer.ClientDataService.UpdateClientAsync(objectId, propertyName, value, callback);
        }
        #endregion

        #region UpdateClientsAsync
        public void UpdateClientsAsync(string propertyName, Dictionary<string, object> values, Action<ResponseBase, Exception> callback) {
            RpcRoot.OfficialServer.ClientDataService.UpdateClientsAsync(propertyName, values, callback);
        }
        #endregion

        #region RemoveClientsAsync
        public void RemoveClientsAsync(List<string> objectIds, Action<ResponseBase, Exception> callback) {
            RpcRoot.OfficialServer.ClientDataService.RemoveClientsAsync(objectIds, callback);
        }
        #endregion

        #region GetLatestSnapshotsAsync
        public void GetLatestSnapshotsAsync(int limit, Action<GetCoinSnapshotsResponse, Exception> callback) {
            RpcRoot.OfficialServer.CoinSnapshotService.GetLatestSnapshotsAsync(limit, callback);
        }
        #endregion

        #region EnableRemoteDesktopAsync
        public void EnableRemoteDesktopAsync(IMinerData client) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.EnableRemoteDesktop) {
                Data = new WrapperClientId {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region GetConsoleOutLinesAsync
        public void GetConsoleOutLinesAsync(IMinerData client, long afterTime) {
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetConsoleOutLines) {
                Data = new WrapperClientIdData {
                    ClientId = client.ClientId,
                    Data = afterTime
                }
            });
        }
        #endregion

        public void GetLocalMessagesAsync(IMinerData client, long afterTime) {
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetLocalMessages) {
                Data = new WrapperClientIdData {
                    ClientId = client.ClientId,
                    Data = afterTime
                }
            });
        }

        #region BlockWAUAsync
        public void BlockWAUAsync(IMinerData client) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.BlockWAU) {
                Data = new WrapperClientId {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region AtikmdagPatcherAsync
        public void AtikmdagPatcherAsync(IMinerData client) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.AtikmdagPatcher) {
                Data = new WrapperClientId {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region SwitchRadeonGpuAsync
        public void SwitchRadeonGpuAsync(IMinerData client, bool on) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.SwitchRadeonGpu) {
                Data = new WrapperClientIdData {
                    ClientId = client.ClientId,
                    Data = on
                }
            });
        }
        #endregion

        #region RestartWindowsAsync
        public void RestartWindowsAsync(IMinerData client) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.RestartWindows) {
                Data = new WrapperClientId {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region ShutdownWindowsAsync
        public void ShutdownWindowsAsync(IMinerData client) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.ShutdownWindows) {
                Data = new WrapperClientId {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region UpgradeNTMinerAsync
        // ReSharper disable once InconsistentNaming
        public void UpgradeNTMinerAsync(IMinerData client, string ntminerFileName) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.UpgradeNTMiner) {
                Data = new WrapperClientIdData {
                    ClientId = client.ClientId,
                    Data = ntminerFileName
                }
            });
        }
        #endregion

        #region SetAutoBootStartAsync
        public void SetAutoBootStartAsync(IMinerData client, SetAutoBootStartRequest request) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.SetAutoBootStart) {
                Data = new WrapperClientIdData {
                    ClientId = client.ClientId,
                    Data = request
                }
            });
        }
        #endregion

        #region StartMineAsync
        public void StartMineAsync(IMinerData client, Guid workId) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            // localJson和serverJson在服务端将消息通过ws通道发送给挖矿端前根据workId填充
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.StartMine) {
                Data = new WrapperClientIdData {
                    ClientId = client.ClientId,
                    Data = workId
                }
            });
        }
        #endregion

        #region StopMineAsync
        public void StopMineAsync(IMinerData client) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.StopMine) {
                Data = new WrapperClientId {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region GetDrivesAsync
        public void GetDrivesAsync(IMinerData client) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetDrives) {
                Data = new WrapperClientId {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region SetVirtualMemoryAsync
        public void SetVirtualMemoryAsync(IMinerData client, Dictionary<string, int> data) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.SetVirtualMemory) {
                Data = new WrapperClientIdData {
                    ClientId = client.ClientId,
                    Data = data
                }
            });
        }
        #endregion

        #region GetLocalIpsAsync
        public void GetLocalIpsAsync(IMinerData client) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetLocalIps) {
                Data = new WrapperClientId {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region SetLocalIpsAsync
        public void SetLocalIpsAsync(IMinerData client, List<LocalIpInput> data) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.SetLocalIps) {
                Data = new WrapperClientIdData {
                    ClientId = client.ClientId,
                    Data = data
                }
            });
        }
        #endregion

        #region GetOperationResultsAsync
        public void GetOperationResultsAsync(IMinerData client, long afterTime) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetOperationResults) {
                Data = new WrapperClientIdData {
                    ClientId = client.ClientId,
                    Data = afterTime
                }
            });
        }
        #endregion

        #region GetSelfWorkLocalJsonAsync
        public void GetSelfWorkLocalJsonAsync(IMinerData client) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetSelfWorkLocalJson) {
                Data = new WrapperClientId {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region SaveSelfWorkLocalJsonAsync
        public void SaveSelfWorkLocalJsonAsync(IMinerData client, string localJson, string serverJson) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            if (string.IsNullOrEmpty(localJson) || string.IsNullOrEmpty(serverJson)) {
                return;
            }
            WorkRequest request = new WorkRequest {
                WorkId = MineWorkData.SelfMineWorkId,
                WorkerName = client.WorkerName,
                LocalJson = localJson.Replace(NTKeyword.MinerNameParameterName, client.WorkerName),
                ServerJson = serverJson
            };
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.SaveSelfWorkLocalJson) {
                Data = new WrapperClientIdData {
                    ClientId = client.ClientId,
                    Data = request
                }
            });
        }
        #endregion

        #region GetGpuProfilesJsonAsync
        public void GetGpuProfilesJsonAsync(IMinerData client) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetGpuProfilesJson) {
                Data = new WrapperClientId {
                    ClientId = client.ClientId
                }
            });
        }
        #endregion

        #region SaveGpuProfilesJsonAsync
        public void SaveGpuProfilesJsonAsync(IMinerData client, string json) {
            if (!MinerStudioRoot.WsClient.IsOpen) {
                VirtualRoot.Out.ShowWarn("和服务器失去连接", autoHideSeconds: 4);
                return;
            }
            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.SaveGpuProfilesJson) {
                Data = new WrapperClientIdData {
                    ClientId = client.ClientId,
                    Data = json
                }
            });
        }
        #endregion
    }
}
