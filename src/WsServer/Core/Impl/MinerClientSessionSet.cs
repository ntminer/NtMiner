using NTMiner.Core.Daemon;
using NTMiner.Ws;
using System;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class MinerClientSessionSet : AbstractSessionSet<IMinerClientSession>, IMinerClientSessionSet {
        public MinerClientSessionSet(IWsSessionsAdapter wsSessions) : base(wsSessions) {
            VirtualRoot.BuildEventPath<GetConsoleOutLinesMqEvent>("收到GetConsoleOutLines Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(GetConsoleOutLinesMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(GetConsoleOutLinesMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetConsoleOutLines) {
                    Data = message.Data
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<GetLocalMessagesMqEvent>("收到GetLocalMessages Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(GetLocalMessagesMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(GetLocalMessagesMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetLocalMessages) {
                    Data = message.Data
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<GetOperationResultsMqEvent>("收到GetOperationResults Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(GetOperationResultsMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(GetOperationResultsMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetOperationResults) {
                    Data = message.Data
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<GetDrivesMqEvent>("收到GetDrives Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(GetDrivesMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(GetDrivesMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetDrives));
                #endregion
            });
            VirtualRoot.BuildEventPath<GetLocalIpsMqEvent>("收到GetLocalIps Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(GetLocalIpsMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(GetLocalIpsMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetLocalIps));
                #endregion
            });
            VirtualRoot.BuildEventPath<GetSpeedMqEvent>("收到GetSpeedMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (message.Requests == null || message.Requests.Length == 0) {
                    return;
                }
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                foreach (var request in message.Requests) {
                    ServerRoot.IfStudioClientTestIdLogElseNothing(request.StudioId, nameof(GetSpeedMqEvent));
                    foreach (var clientId in request.ClientIds.Where(a => TryGetByClientId(a, out _))) {
                        if (!IsOwnerBy(clientId, request.LoginName, message.Timestamp)) {
                            continue;
                        }
                        ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, nameof(GetSpeedMqEvent));
                        SendToMinerClientAsync(clientId, new WsMessage(message.MessageId, WsMessage.GetSpeed));
                    }
                }
                #endregion
            });
            VirtualRoot.BuildEventPath<EnableRemoteDesktopMqEvent>("收到EnableRemoteDesktopMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(EnableRemoteDesktopMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(EnableRemoteDesktopMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.EnableRemoteDesktop));
                #endregion
            });
            VirtualRoot.BuildEventPath<BlockWAUMqEvent>("收到BlockWAUMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(BlockWAUMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(BlockWAUMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.BlockWAU));
                #endregion
            });
            VirtualRoot.BuildEventPath<SetVirtualMemoryMqEvent>("收到SetVirtualMemoryMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(SetVirtualMemoryMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(SetVirtualMemoryMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SetVirtualMemory) {
                    Data = message.Data
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<SetLocalIpsMqEvent>("收到SetLocalIpsMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(SetLocalIpsMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(SetLocalIpsMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SetLocalIps) {
                    Data = message.Data
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<SwitchRadeonGpuMqEvent>("收到SwitchRadeonGpuMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(SwitchRadeonGpuMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(SwitchRadeonGpuMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SwitchRadeonGpu) {
                    Data = message.On
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<GetSelfWorkLocalJsonMqEvent>("收到GetSelfWorkLocalJsonMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(GetSelfWorkLocalJsonMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(GetSelfWorkLocalJsonMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetSelfWorkLocalJson));
                #endregion
            });
            VirtualRoot.BuildEventPath<SaveSelfWorkLocalJsonMqEvent>("收到SaveSelfWorkLocalJsonMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(SaveSelfWorkLocalJsonMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(SaveSelfWorkLocalJsonMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SaveSelfWorkLocalJson) {
                    Data = message.Data
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<GetGpuProfilesJsonMqEvent>("收到GetGpuProfilesJsonMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(GetGpuProfilesJsonMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(GetGpuProfilesJsonMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetGpuProfilesJson));
                #endregion
            });
            VirtualRoot.BuildEventPath<SaveGpuProfilesJsonMqEvent>("收到SaveGpuProfilesJsonMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(SaveGpuProfilesJsonMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(SaveGpuProfilesJsonMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SaveGpuProfilesJson) {
                    Data = message.Data
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<SetAutoBootStartMqEvent>("收到SetAutoBootStartMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(SetAutoBootStartMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(SetAutoBootStartMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SetAutoBootStart) {
                    Data = message.Data
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<RestartWindowsMqEvent>("收到RestartWindowsMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(RestartWindowsMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(RestartWindowsMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.RestartWindows));
                #endregion
            });
            VirtualRoot.BuildEventPath<ShutdownWindowsMqEvent>("收到ShutdownWindowsMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(ShutdownWindowsMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(ShutdownWindowsMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.ShutdownWindows));
                #endregion
            });
            VirtualRoot.BuildEventPath<UpgradeNTMinerMqEvent>("收到UpgradeNTMinerMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(UpgradeNTMinerMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(UpgradeNTMinerMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.UpgradeNTMiner) {
                    Data = message.Data
                });
                #endregion
            });
            // WsServer节点和WebApiServer节点都订阅了该消息，WsServer节点只处理非作业消息，WebApiServer节点只处理作业消息
            VirtualRoot.BuildEventPath<StartMineMqEvent>("收到StartMineMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(StartMineMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(StartMineMqEvent));
                Guid workId = message.Data;
                // 只处理非作业的
                if (workId == Guid.Empty) {
                    SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.StartMine) {
                        Data = new WorkRequest {
                            WorkId = workId,
                            WorkerName = string.Empty,
                            LocalJson = string.Empty,
                            ServerJson = string.Empty
                        }
                    });
                }
                #endregion
            });
            // WebApiServer节点订阅了StartMineMqMessage消息，当StartMineMqMessage消息是作业消息时WebApiServer节点重新广播StartWorkMineMqMessage消息
            VirtualRoot.BuildEventPath<StartWorkMineMqEvent>("收到StartWorkMineMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(StartWorkMineMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(StartWorkMineMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.StartMine) {
                    Data = message.Data
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<StopMineMqEvent>("收到StopMineMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(StopMineMqEvent));
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(StopMineMqEvent));
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.StopMine));
                #endregion
            });
        }

        public void SendToMinerClientAsync(Guid clientId, WsMessage message) {
            if (TryGetByClientId(clientId, out IMinerClientSession minerClientSession)) {
                ServerRoot.IfMinerClientTestIdLogElseNothing(minerClientSession.ClientId, $"{nameof(WsMessage)}.{message.Type}");
                minerClientSession.SendAsync(message);
            }
        }
    }
}
