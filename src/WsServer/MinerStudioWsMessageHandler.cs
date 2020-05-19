using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static class MinerStudioWsMessageHandler {
        private static readonly Dictionary<string, Action<string, WsMessage>>
            _handlers = new Dictionary<string, Action<string, WsMessage>>(StringComparer.OrdinalIgnoreCase) {
                [WsMessage.GetConsoleOutLines] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        WsRoot.OperationMqSender.SendGetConsoleOutLines(loginName, wrapperClientIdData.ClientId, afterTime);
                    }
                },
                [WsMessage.GetLocalMessages] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        WsRoot.OperationMqSender.SendGetLocalMessages(loginName, wrapperClientIdData.ClientId, afterTime);
                    }
                },
                [WsMessage.GetDrives] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        WsRoot.OperationMqSender.SendGetDrives(loginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.GetLocalIps] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        WsRoot.OperationMqSender.SendGetLocalIps(loginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.GetOperationResults] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        WsRoot.OperationMqSender.SendGetOperationResults(loginName, wrapperClientIdData.ClientId, afterTime);
                    }
                },
                [WsMessage.GetSpeed] = (loginName, message) => {
                    if (message.TryGetData(out List<Guid> minerIds)) {
                        WsRoot.OperationMqSender.SendGetSpeed(loginName, minerIds);
                    }
                },
                [WsMessage.EnableRemoteDesktop] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        WsRoot.OperationMqSender.SendEnableRemoteDesktop(loginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.BlockWAU] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        WsRoot.OperationMqSender.SendBlockWAU(loginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.SetVirtualMemory] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out Dictionary<string, int> data)) {
                        WsRoot.OperationMqSender.SendSetVirtualMemory(loginName, wrapperClientIdData.ClientId, data);
                    }
                },
                [WsMessage.SetLocalIps] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<LocalIpInput> data)) {
                        WsRoot.OperationMqSender.SendSetLocalIps(loginName, wrapperClientIdData.ClientId, data);
                    }
                },
                [WsMessage.SwitchRadeonGpu] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out bool on)) {
                        WsRoot.OperationMqSender.SendSwitchRadeonGpu(loginName, wrapperClientIdData.ClientId, on);
                    }
                },
                [WsMessage.GetSelfWorkLocalJson] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        WsRoot.OperationMqSender.SendGetSelfWorkLocalJson(loginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.SaveSelfWorkLocalJson] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out WorkRequest workRequest)) {
                        WsRoot.OperationMqSender.SendSaveSelfWorkLocalJson(loginName, wrapperClientData.ClientId, workRequest);
                    }
                },
                [WsMessage.GetGpuProfilesJson] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        WsRoot.OperationMqSender.SendGetGpuProfilesJson(loginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.SaveGpuProfilesJson] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out string json)) {
                        WsRoot.OperationMqSender.SendSaveGpuProfilesJson(loginName, wrapperClientData.ClientId, json);
                    }
                },
                [WsMessage.SetAutoBootStart] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out SetAutoBootStartRequest body)) {
                        WsRoot.OperationMqSender.SendSetAutoBootStart(loginName, wrapperClientData.ClientId, body);
                    }
                },
                [WsMessage.RestartWindows] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        WsRoot.OperationMqSender.SendRestartWindows(loginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.ShutdownWindows] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        WsRoot.OperationMqSender.SendShutdownWindows(loginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.UpgradeNTMiner] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out string ntminerFileName)) {
                        WsRoot.OperationMqSender.SendUpgradeNTMiner(loginName, wrapperClientData.ClientId, ntminerFileName);
                    }
                },
                [WsMessage.StartMine] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out Guid workId)) {
                        WsRoot.OperationMqSender.SendStartMine(loginName, wrapperClientData.ClientId, workId);
                    }
                },
                [WsMessage.StopMine] = (loginName, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        WsRoot.OperationMqSender.SendStopMine(loginName, wrapperClientId.ClientId);
                    }
                }
            };

        public static bool TryGetHandler(string wsMessageType, out Action<string, WsMessage> handler) {
            return _handlers.TryGetValue(wsMessageType, out handler);
        }
    }
}
