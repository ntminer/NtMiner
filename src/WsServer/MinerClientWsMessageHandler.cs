using NTMiner.Core;
using NTMiner.Core.MinerClient;
using NTMiner.Report;
using NTMiner.VirtualMemory;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static class MinerClientWsMessageHandler {
        private static readonly Dictionary<string, Action<IWsSessionAdapter, string, Guid, WsMessage>>
            _handlers = new Dictionary<string, Action<IWsSessionAdapter, string, Guid, WsMessage>>(StringComparer.OrdinalIgnoreCase) {
                [WsMessage.ConsoleOutLines] = (webSocketSession, loginName, clientId, message) => {
                    if (message.TryGetData(out List<ConsoleOutLine> consoleOutLines)) {
                        WsRoot.OperationMqSender.SendConsoleOutLines(loginName, clientId, consoleOutLines);
                    }
                },
                [WsMessage.LocalMessages] = (webSocketSession, loginName, clientId, message) => {
                    if (message.TryGetData(out List<LocalMessageDto> datas)) {
                        WsRoot.OperationMqSender.SendLocalMessages(loginName, clientId, datas);
                    }
                },
                [WsMessage.Drives] = (webSocketSession, loginName, clientId, message) => {
                    if (message.TryGetData(out List<DriveDto> datas)) {
                        WsRoot.OperationMqSender.SendDrives(loginName, clientId, datas);
                    }
                },
                [WsMessage.LocalIps] = (webSocketSession, loginName, clientId, message) => {
                    if (message.TryGetData(out List<LocalIpDto> datas)) {
                        WsRoot.OperationMqSender.SendLocalIps(loginName, clientId, datas);
                    }
                },
                [WsMessage.OperationResults] = (webSocketSession, loginName, clientId, message) => {
                    if (message.TryGetData(out List<OperationResultData> datas)) {
                        WsRoot.OperationMqSender.SendOperationResults(loginName, clientId, datas);
                    }
                },
                [WsMessage.OperationReceived] = (webSocketSession, loginName, clientId, message) => {
                    WsRoot.OperationMqSender.SendOperationReceived(loginName, clientId);
                },
                [WsMessage.Speed] = (webSocketSession, loginName, clientId, message) => {
                    if (message.TryGetData(out SpeedDto speedDto)) {
                        WsRoot.SpeedDataRedis.SetAsync(new SpeedData(speedDto, DateTime.Now)).ContinueWith(t => {
                            WsRoot.MinerClientMqSender.SendSpeed(loginName, speedDto.ClientId, webSocketSession.RemoteEndPoint.ToString());
                        });
                    }
                },
                [WsMessage.SelfWorkLocalJson] = (webSocketSession, loginName, clientId, message) => {
                    if (message.TryGetData(out string json)) {
                        WsRoot.OperationMqSender.SendSelfWorkLocalJson(loginName, clientId, json);
                    }
                },
                [WsMessage.GpuProfilesJson] = (webSocketSession, loginName, clientId, message) => {
                    if (message.TryGetData(out string json)) {
                        WsRoot.OperationMqSender.SendGpuProfilesJson(loginName, clientId, json);
                    }
                }
            };

        public static bool TryGetHandler(string wsMessageType, out Action<IWsSessionAdapter, string, Guid, WsMessage> handler) {
            return _handlers.TryGetValue(wsMessageType, out handler);
        }
    }
}
