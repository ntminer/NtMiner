using NTMiner.Core;
using NTMiner.Core.MinerClient;
using NTMiner.Core.Mq;
using NTMiner.Report;
using NTMiner.VirtualMemory;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static class WsMessageFromMinerClientHandler {
        static WsMessageFromMinerClientHandler() {
        }

        private static readonly Dictionary<string, Action<IMinerClientSession, Guid, WsMessage>>
            _handlers = new Dictionary<string, Action<IMinerClientSession, Guid, WsMessage>>(StringComparer.OrdinalIgnoreCase) {
                [WsMessage.ConsoleOutLines] = (session, clientId, message) => {
                    if (message.TryGetData(out List<ConsoleOutLine> consoleOutLines)) {
                        if (MqBufferRoot.TryRemoveFastId(message.Id)) {
                            AppRoot.OperationMqSender.SendConsoleOutLines(session.LoginName, clientId, consoleOutLines);
                        }
                        else {
                            // TODO:
                        }
                    }
                },
                [WsMessage.LocalMessages] = (session, clientId, message) => {
                    if (message.TryGetData(out List<LocalMessageDto> localMessages)) {
                        if (MqBufferRoot.TryRemoveFastId(message.Id)) {
                            AppRoot.OperationMqSender.SendLocalMessages(session.LoginName, clientId, localMessages);
                        }
                        else {
                            // TODO:
                        }
                    }
                },
                [WsMessage.OperationResults] = (session, clientId, message) => {
                    if (message.TryGetData(out List<OperationResultData> operationResults)) {
                        if (MqBufferRoot.TryRemoveFastId(message.Id)) {
                            AppRoot.OperationMqSender.SendOperationResults(session.LoginName, clientId, operationResults);
                        }
                        else {
                            // TODO:
                        }
                    }
                },
                [WsMessage.Drives] = (session, clientId, message) => {
                    if (message.TryGetData(out List<DriveDto> drives)) {
                        AppRoot.OperationMqSender.SendDrives(session.LoginName, clientId, drives);
                    }
                },
                [WsMessage.LocalIps] = (session, clientId, message) => {
                    if (message.TryGetData(out List<LocalIpDto> localIps)) {
                        AppRoot.OperationMqSender.SendLocalIps(session.LoginName, clientId, localIps);
                    }
                },
                [WsMessage.OperationReceived] = (session, clientId, message) => {
                    AppRoot.OperationMqSender.SendOperationReceived(session.LoginName, clientId);
                },
                [WsMessage.Speed] = (session, clientId, message) => {
                    if (message.TryGetData(out SpeedDto speedDto)) {
                        AppRoot.SpeedDataRedis.SetAsync(new SpeedData(speedDto, DateTime.Now)).ContinueWith(t => {
                            MqBufferRoot.SendSpeed(new ClientIdIp(speedDto.ClientId, session.RemoteEndPoint.ToString()));
                        });
                    }
                },
                [WsMessage.SelfWorkLocalJson] = (session, clientId, message) => {
                    if (message.TryGetData(out string json)) {
                        AppRoot.OperationMqSender.SendSelfWorkLocalJson(session.LoginName, clientId, json);
                    }
                },
                [WsMessage.GpuProfilesJson] = (session, clientId, message) => {
                    if (message.TryGetData(out string json)) {
                        AppRoot.OperationMqSender.SendGpuProfilesJson(session.LoginName, clientId, json);
                    }
                }
            };

        public static bool TryGetHandler(string wsMessageType, out Action<IMinerClientSession, Guid, WsMessage> handler) {
            return _handlers.TryGetValue(wsMessageType, out handler);
        }
    }
}
