using NTMiner.Core;
using NTMiner.Core.MinerClient;
using NTMiner.Report;
using NTMiner.VirtualMemory;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static class MinerClientWsMessageHandler {
        private static readonly Dictionary<string, Action<IMinerClientSession, Guid, WsMessage>>
            _handlers = new Dictionary<string, Action<IMinerClientSession, Guid, WsMessage>>(StringComparer.OrdinalIgnoreCase) {
                [WsMessage.ConsoleOutLines] = (session, clientId, message) => {
                    if (message.TryGetData(out List<ConsoleOutLine> consoleOutLines)) {
                        AppRoot.OperationMqSender.SendConsoleOutLines(session.LoginName, clientId, consoleOutLines);
                    }
                },
                [WsMessage.LocalMessages] = (session, clientId, message) => {
                    if (message.TryGetData(out List<LocalMessageDto> datas)) {
                        AppRoot.OperationMqSender.SendLocalMessages(session.LoginName, clientId, datas);
                    }
                },
                [WsMessage.Drives] = (session, clientId, message) => {
                    if (message.TryGetData(out List<DriveDto> datas)) {
                        AppRoot.OperationMqSender.SendDrives(session.LoginName, clientId, datas);
                    }
                },
                [WsMessage.LocalIps] = (session, clientId, message) => {
                    if (message.TryGetData(out List<LocalIpDto> datas)) {
                        AppRoot.OperationMqSender.SendLocalIps(session.LoginName, clientId, datas);
                    }
                },
                [WsMessage.OperationResults] = (session, clientId, message) => {
                    if (message.TryGetData(out List<OperationResultData> datas)) {
                        AppRoot.OperationMqSender.SendOperationResults(session.LoginName, clientId, datas);
                    }
                },
                [WsMessage.OperationReceived] = (session, clientId, message) => {
                    AppRoot.OperationMqSender.SendOperationReceived(session.LoginName, clientId);
                },
                [WsMessage.Speed] = (session, clientId, message) => {
                    if (message.TryGetData(out SpeedDto speedDto)) {
                        AppRoot.SpeedDataRedis.SetAsync(new SpeedData(speedDto, DateTime.Now)).ContinueWith(t => {
                            AppRoot.MinerClientMqSender.SendSpeed(session.LoginName, speedDto.ClientId, session.RemoteEndPoint.ToString());
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
