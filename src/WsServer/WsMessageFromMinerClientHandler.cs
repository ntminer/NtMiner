using NTMiner.Core;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
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
                    if (message.TryGetData(out List<ConsoleOutLine> consoleOutLines) && consoleOutLines != null && consoleOutLines.Count != 0) {
                        if (MqBufferRoot.TryRemoveFastId(message.Id)) {
                            ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"fast {nameof(WsMessage)}.{message.Type}");
                            AppRoot.OperationMqSender.SendConsoleOutLines(session.LoginName, clientId, consoleOutLines);
                        }
                        else {
                            ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"{nameof(WsMessage)}.{message.Type}");
                            MqBufferRoot.ConsoleOutLines(new ConsoleOutLines {
                                LoginName = session.LoginName,
                                ClientId = clientId,
                                Data = consoleOutLines
                            });
                        }
                    }
                },
                [WsMessage.LocalMessages] = (session, clientId, message) => {
                    if (message.TryGetData(out List<LocalMessageDto> localMessages) && localMessages != null && localMessages.Count != 0) {
                        if (MqBufferRoot.TryRemoveFastId(message.Id)) {
                            ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"fast {nameof(WsMessage)}.{message.Type}");
                            AppRoot.OperationMqSender.SendLocalMessages(session.LoginName, clientId, localMessages);
                        }
                        else {
                            ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"{nameof(WsMessage)}.{message.Type}");
                            MqBufferRoot.LocalMessages(new LocalMessages {
                                LoginName = session.LoginName,
                                ClientId = clientId,
                                Data = localMessages
                            });
                        }
                    }
                },
                [WsMessage.OperationResults] = (session, clientId, message) => {
                    if (message.TryGetData(out List<OperationResultData> operationResults) && operationResults != null && operationResults.Count != 0) {
                        if (MqBufferRoot.TryRemoveFastId(message.Id)) {
                            ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"fast {nameof(WsMessage)}.{message.Type}");
                            AppRoot.OperationMqSender.SendOperationResults(session.LoginName, clientId, operationResults);
                        }
                        else {
                            ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"{nameof(WsMessage)}.{message.Type}");
                            MqBufferRoot.OperationResults(new OperationResults {
                                LoginName = session.LoginName,
                                ClientId = clientId,
                                Data = operationResults
                            });
                        }
                    }
                },
                [WsMessage.Drives] = (session, clientId, message) => {
                    if (message.TryGetData(out List<DriveDto> drives)) {
                        ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"{nameof(WsMessage)}.{message.Type}");
                        AppRoot.OperationMqSender.SendDrives(session.LoginName, clientId, drives);
                    }
                },
                [WsMessage.LocalIps] = (session, clientId, message) => {
                    if (message.TryGetData(out List<LocalIpDto> localIps)) {
                        ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"{nameof(WsMessage)}.{message.Type}");
                        AppRoot.OperationMqSender.SendLocalIps(session.LoginName, clientId, localIps);
                    }
                },
                [WsMessage.OperationReceived] = (session, clientId, message) => {
                    ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"{nameof(WsMessage)}.{message.Type}");
                    AppRoot.OperationMqSender.SendOperationReceived(session.LoginName, clientId);
                },
                [WsMessage.Speed] = (session, clientId, message) => {
                    if (message.TryGetData(out SpeedDto speedDto)) {
                        ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"{nameof(WsMessage)}.{message.Type}");
                        AppRoot.SpeedDataRedis.SetAsync(new SpeedData(speedDto, DateTime.Now)).ContinueWith(t => {
                            MqBufferRoot.SendSpeed(new ClientIdIp(speedDto.ClientId, session.RemoteEndPoint.ToString()));
                        });
                    }
                },
                [WsMessage.SelfWorkLocalJson] = (session, clientId, message) => {
                    if (message.TryGetData(out string json)) {
                        ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"{nameof(WsMessage)}.{message.Type}");
                        AppRoot.OperationMqSender.SendSelfWorkLocalJson(session.LoginName, clientId, json);
                    }
                },
                [WsMessage.GpuProfilesJson] = (session, clientId, message) => {
                    if (message.TryGetData(out string json)) {
                        ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"{nameof(WsMessage)}.{message.Type}");
                        AppRoot.OperationMqSender.SendGpuProfilesJson(session.LoginName, clientId, json);
                    }
                },
                [WsMessage.CalcConfigs] = (session, clientId, message) => {
                    ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"{nameof(WsMessage)}.{message.Type}");
                    List<CalcConfigData> data = AppRoot.CalcConfigSet.Gets(string.Empty);
                    session.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.CalcConfigs) {
                        Data = data
                    });
                },
                [WsMessage.QueryCalcConfigs] = (session, clientId, message) => {
                    if (message.TryGetData(out string coinCodes)) {
                        ServerRoot.IfMinerClientTestIdLogElseNothing(clientId, $"{nameof(WsMessage)}.{message.Type}");
                        List<CalcConfigData> data = AppRoot.CalcConfigSet.Gets(coinCodes);
                        session.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.CalcConfigs) {
                            Data = data
                        });
                    }
                }
            };

        public static bool TryGetHandler(string wsMessageType, out Action<IMinerClientSession, Guid, WsMessage> handler) {
            return _handlers.TryGetValue(wsMessageType, out handler);
        }
    }
}
