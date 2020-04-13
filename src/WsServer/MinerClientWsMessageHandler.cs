using NTMiner.Core;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.VirtualMemory;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static class MinerClientWsMessageHandler {
        private static readonly Dictionary<string, Action<MinerClientBehavior, string, Guid, WsMessage>> 
            _handlers = new Dictionary<string, Action<MinerClientBehavior, string, Guid, WsMessage>>(StringComparer.OrdinalIgnoreCase) {
            {WsMessage.ConsoleOutLines,
                (wsBehavior, loginName, clientId, message) => {
                    if (message.TryGetData(out List<ConsoleOutLine> consoleOutLines)) {
                        WsRoot.OperationMqSender.SendConsoleOutLines(loginName, clientId, consoleOutLines);
                    }
                }
            },
            {WsMessage.LocalMessages,
                (wsBehavior, loginName, clientId, message) => {
                    if (message.TryGetData(out List<LocalMessageDto> datas)) {
                        WsRoot.OperationMqSender.SendLocalMessages(loginName, clientId, datas);
                    }
                }
            },
            {WsMessage.Drives,
                (wsBehavior, loginName, clientId, message) => {
                    if (message.TryGetData(out List<DriveDto> datas)) {
                        WsRoot.OperationMqSender.SendDrives(loginName, clientId, datas);
                    }
                }
            },
            {WsMessage.LocalIps,
                (wsBehavior, loginName, clientId, message) => {
                    if (message.TryGetData(out List<LocalIpDto> datas)) {
                        WsRoot.OperationMqSender.SendLocalIps(loginName, clientId, datas);
                    }
                }
            },
            {WsMessage.OperationResults,
                (wsBehavior, loginName, clientId, message) => {
                    if (message.TryGetData(out List<OperationResultData> datas)) {
                        WsRoot.OperationMqSender.SendOperationResults(loginName, clientId, datas);
                    }
                }
            },
            {WsMessage.OperationReceived,
                (wsBehavior, loginName, clientId, message) => {
                    WsRoot.OperationMqSender.SendOperationReceived(loginName, clientId);
                }
            },
            {WsMessage.Speed,
                (wsBehavior, loginName, clientId, message) => {
                    if (message.TryGetData(out SpeedData speedData)) {
                        WsRoot.OperationMqSender.SendSpeedData(loginName, speedData, wsBehavior.Context.UserEndPoint.ToString());
                    }
                }
            },
            {WsMessage.GpuProfilesJson,
                (wsBehavior, loginName, clientId, message) => {
                    if (message.TryGetData(out string json)) {
                        WsRoot.OperationMqSender.SendGpuProfilesJson(loginName, clientId, json);
                    }
                }
            }
        };

        public static bool TryGetHandler(string wsMessageType, out Action<MinerClientBehavior, string, Guid, WsMessage> handler) {
            return _handlers.TryGetValue(wsMessageType, out handler);
        }
    }
}
