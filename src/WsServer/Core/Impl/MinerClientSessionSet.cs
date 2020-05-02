using NTMiner.Core.Daemon;
using NTMiner.Core.MinerServer;
using NTMiner.User;
using NTMiner.Ws;
using System;
using WebSocketSharp.Server;

namespace NTMiner.Core.Impl {
    public class MinerClientSessionSet : AbstractSessionSet<IMinerClientSession>, IMinerClientSessionSet {
        public MinerClientSessionSet() : base(MinerClientBehavior.WsServiceHostPath) {
            VirtualRoot.AddEventPath<GetConsoleOutLinesMqMessage>("收到GetConsoleOutLines Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetConsoleOutLines) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<GetLocalMessagesMqMessage>("收到GetLocalMessages Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetLocalMessages) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<GetDrivesMqMessage>("收到GetDrives Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetDrives));
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<GetLocalIpsMqMessage>("收到GetLocalIps Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetLocalIps));
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<GetOperationResultsMqMessage>("收到GetOperationResults Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetOperationResults) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<GetSpeedMqMessage>("收到GetSpeedMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (message.ClientIds == null || message.ClientIds.Count == 0) {
                    return;
                }
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                IUser user = WsRoot.ReadOnlyUserSet.GetUser(UserId.CreateLoginNameUserId(message.LoginName));
                if (user == null) {
                    return;
                }
                foreach (var clientId in message.ClientIds) {
                    if (clientId == null || clientId == Guid.Empty) {
                        continue;
                    }
                    if (!WsRoot.MinerSignSet.TryGetByClientId(clientId, out MinerSign minerSign) || !minerSign.IsOwnerBy(user)) {
                        continue;
                    }
                    SendToMinerClientAsync(clientId, new WsMessage(message.MessageId, WsMessage.GetSpeed));
                }
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<EnableRemoteDesktopMqMessage>("收到EnableRemoteDesktopMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.EnableRemoteDesktop));
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<BlockWAUMqMessage>("收到BlockWAUMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.BlockWAU));
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<SetVirtualMemoryMqMessage>("收到SetVirtualMemoryMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SetVirtualMemory) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<SetLocalIpsMqMessage>("收到SetLocalIpsMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SetLocalIps) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<AtikmdagPatcherMqMessage>("收到AtikmdagPatcherMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.AtikmdagPatcher));
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<SwitchRadeonGpuMqMessage>("收到SwitchRadeonGpuMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SwitchRadeonGpu) {
                    Data = message.On
                });
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<GetSelfWorkLocalJsonMqMessage>("收到GetSelfWorkLocalJsonMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetSelfWorkLocalJson));
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<SaveSelfWorkLocalJsonMqMessage>("收到SaveSelfWorkLocalJsonMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SaveSelfWorkLocalJson) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<GetGpuProfilesJsonMqMessage>("收到GetGpuProfilesJsonMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetGpuProfilesJson));
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<SaveGpuProfilesJsonMqMessage>("收到SaveGpuProfilesJsonMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SaveGpuProfilesJson) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<SetAutoBootStartMqMessage>("收到SetAutoBootStartMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SetAutoBootStart) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<RestartWindowsMqMessage>("收到RestartWindowsMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.RestartWindows));
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<ShutdownWindowsMqMessage>("收到ShutdownWindowsMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.ShutdownWindows));
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<UpgradeNTMinerMqMessage>("收到UpgradeNTMinerMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.UpgradeNTMiner) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<StartMineMqMessage>("收到StartMineMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                if (message.Data != Guid.Empty) {
                    RpcRoot.OfficialServer.UserMineWorkService.GetWorkJsonAsync(message.Data, message.ClientId, (response, e) => {
                        if (response.IsSuccess()) {
                            SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.StartMine) {
                                Data = new WorkRequest {
                                    WorkId = message.Data,
                                    WorkerName = response.WorkerName,
                                    LocalJson = response.LocalJson,
                                    ServerJson = response.ServerJson
                                }
                            });
                        }
                    });
                }
                else {
                    SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.StartMine) {
                        Data = new WorkRequest {
                            WorkId = message.Data,
                            WorkerName = string.Empty,
                            LocalJson = string.Empty,
                            ServerJson = string.Empty
                        }
                    });
                }
                #endregion
            }, this.GetType());
            VirtualRoot.AddEventPath<StopMineMqMessage>("收到StopMineMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, action: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.StopMine));
                #endregion
            }, this.GetType());
        }

        public void SendToMinerClientAsync(Guid clientId, WsMessage message) {
            if (TryGetByClientId(clientId, out IMinerClientSession minerClientSession) && minerClientSession.TryGetWsSession(out IWebSocketSession wsSession)) {
                string password = minerClientSession.GetSignPassword();
                try {
                    wsSession.Context.WebSocket.SendAsync(message.SignToJson(password), completed: null);
                }
                catch {
                }
            }
        }
    }
}
