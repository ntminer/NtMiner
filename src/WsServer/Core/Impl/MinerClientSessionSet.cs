using NTMiner.Core.Daemon;
using NTMiner.Ws;
using System;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class MinerClientSessionSet : AbstractSessionSet<IMinerClientSession>, IMinerClientSessionSet {
        public MinerClientSessionSet(IWsSessionsAdapter wsSessions) : base(wsSessions) {
            VirtualRoot.BuildEventPath<GetConsoleOutLinesMqEvent>("收到GetConsoleOutLines Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(GetConsoleOutLinesMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(GetConsoleOutLinesMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetConsoleOutLines) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<GetLocalMessagesMqEvent>("收到GetLocalMessages Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(GetLocalMessagesMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(GetLocalMessagesMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetLocalMessages) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<GetOperationResultsMqEvent>("收到GetOperationResults Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(GetOperationResultsMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(GetOperationResultsMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetOperationResults) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<GetDrivesMqEvent>("收到GetDrives Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(GetDrivesMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(GetDrivesMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetDrives));
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<GetLocalIpsMqEvent>("收到GetLocalIps Mq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(GetLocalIpsMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(GetLocalIpsMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetLocalIps));
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<GetSpeedMqEvent>("收到GetSpeedMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (message.Requests == null || message.Requests.Length == 0) {
                    return;
                }
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                foreach (var request in message.Requests) {
                    if (ServerRoot.IsStudioClientTestId(request.StudioId)) {
                        Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {request.StudioId.ToString()} {nameof(GetSpeedMqEvent)}");
                    }
                    foreach (var clientId in request.ClientIds.Where(a => TryGetByClientId(a, out _))) {
                        if (!IsOwnerBy(clientId, request.LoginName, message.Timestamp)) {
                            continue;
                        }
                        if (ServerRoot.IsMinerClientTestId(clientId)) {
                            Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} {nameof(GetSpeedMqEvent)}");
                        }
                        SendToMinerClientAsync(clientId, new WsMessage(message.MessageId, WsMessage.GetSpeed));
                    }
                }
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<EnableRemoteDesktopMqEvent>("收到EnableRemoteDesktopMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(EnableRemoteDesktopMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(EnableRemoteDesktopMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.EnableRemoteDesktop));
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<BlockWAUMqEvent>("收到BlockWAUMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(BlockWAUMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(BlockWAUMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.BlockWAU));
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<SetVirtualMemoryMqEvent>("收到SetVirtualMemoryMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(SetVirtualMemoryMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(SetVirtualMemoryMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SetVirtualMemory) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<SetLocalIpsMqEvent>("收到SetLocalIpsMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(SetLocalIpsMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(SetLocalIpsMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SetLocalIps) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<SwitchRadeonGpuMqEvent>("收到SwitchRadeonGpuMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(SwitchRadeonGpuMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(SwitchRadeonGpuMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SwitchRadeonGpu) {
                    Data = message.On
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<GetSelfWorkLocalJsonMqEvent>("收到GetSelfWorkLocalJsonMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(GetSelfWorkLocalJsonMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(GetSelfWorkLocalJsonMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetSelfWorkLocalJson));
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<SaveSelfWorkLocalJsonMqEvent>("收到SaveSelfWorkLocalJsonMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(SaveSelfWorkLocalJsonMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(SaveSelfWorkLocalJsonMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SaveSelfWorkLocalJson) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<GetGpuProfilesJsonMqEvent>("收到GetGpuProfilesJsonMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(GetGpuProfilesJsonMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(GetGpuProfilesJsonMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.GetGpuProfilesJson));
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<SaveGpuProfilesJsonMqEvent>("收到SaveGpuProfilesJsonMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(SaveGpuProfilesJsonMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(SaveGpuProfilesJsonMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SaveGpuProfilesJson) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<SetAutoBootStartMqEvent>("收到SetAutoBootStartMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(SetAutoBootStartMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(SetAutoBootStartMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.SetAutoBootStart) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<RestartWindowsMqEvent>("收到RestartWindowsMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(RestartWindowsMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(RestartWindowsMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.RestartWindows));
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<ShutdownWindowsMqEvent>("收到ShutdownWindowsMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(ShutdownWindowsMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(ShutdownWindowsMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.ShutdownWindows));
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<UpgradeNTMinerMqEvent>("收到UpgradeNTMinerMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(UpgradeNTMinerMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(UpgradeNTMinerMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.UpgradeNTMiner) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            // WsServer节点和WebApiServer节点都订阅了该消息，WsServer节点只处理非作业消息，WebApiServer节点只处理作业消息
            VirtualRoot.BuildEventPath<StartMineMqEvent>("收到StartMineMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(StartMineMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(StartMineMqEvent)}");
                }
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
            }, this.GetType());
            // WebApiServer节点订阅了StartMineMqMessage消息，当StartMineMqMessage消息是作业消息时WebApiServer节点重新广播StartWorkMineMqMessage消息
            VirtualRoot.BuildEventPath<StartWorkMineMqEvent>("收到StartWorkMineMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(StartWorkMineMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(StartWorkMineMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.StartMine) {
                    Data = message.Data
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<StopMineMqEvent>("收到StopMineMq消息后检查是否是应由本节点处理的消息，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsOwnerBy(message.ClientId, message.LoginName, message.Timestamp)) {
                    return;
                }
                if (ServerRoot.IsMinerClientTestId(message.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {message.ClientId.ToString()} {nameof(StopMineMqEvent)}");
                }
                if (ServerRoot.IsStudioClientTestId(message.StudioId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {message.StudioId.ToString()} {nameof(StopMineMqEvent)}");
                }
                SendToMinerClientAsync(message.ClientId, new WsMessage(message.MessageId, WsMessage.StopMine));
                #endregion
            }, this.GetType());
        }

        public void SendToMinerClientAsync(Guid clientId, WsMessage message) {
            if (TryGetByClientId(clientId, out IMinerClientSession minerClientSession)) {
                if (ServerRoot.IsMinerClientTestId(minerClientSession.ClientId)) {
                    Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {minerClientSession.ClientId.ToString()} {nameof(WsMessage)}.{message.Type}");
                }
                string password = minerClientSession.GetSignPassword();
                minerClientSession.SendAsync(message, password);
            }
        }
    }
}
