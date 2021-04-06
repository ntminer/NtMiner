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
                    if (message.TryGetData(out List<ConsoleOutLine> consoleOutLines) && consoleOutLines != null && consoleOutLines.Count != 0) {
                        if (MqBufferRoot.TryRemoveFastId(message.Id)) {
                            if (ServerRoot.IsMinerClientTestId(clientId)) {
                                Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} fast {nameof(WsMessage)}.{nameof(WsMessage.ConsoleOutLines)}");
                            }
                            AppRoot.OperationMqSender.SendConsoleOutLines(session.LoginName, clientId, consoleOutLines);
                        }
                        else {
                            if (ServerRoot.IsMinerClientTestId(clientId)) {
                                Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} {nameof(WsMessage)}.{nameof(WsMessage.ConsoleOutLines)}");
                            }
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
                            if (ServerRoot.IsMinerClientTestId(clientId)) {
                                Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} fast {nameof(WsMessage)}.{nameof(WsMessage.LocalMessages)}");
                            }
                            AppRoot.OperationMqSender.SendLocalMessages(session.LoginName, clientId, localMessages);
                        }
                        else {
                            if (ServerRoot.IsMinerClientTestId(clientId)) {
                                Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} {nameof(WsMessage)}.{nameof(WsMessage.LocalMessages)}");
                            }
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
                            if (ServerRoot.IsMinerClientTestId(clientId)) {
                                Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} fast {nameof(WsMessage)}.{nameof(WsMessage.OperationResults)}");
                            }
                            AppRoot.OperationMqSender.SendOperationResults(session.LoginName, clientId, operationResults);
                        }
                        else {
                            if (ServerRoot.IsMinerClientTestId(clientId)) {
                                Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} {nameof(WsMessage)}.{nameof(WsMessage.OperationResults)}");
                            }
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
                        if (ServerRoot.IsMinerClientTestId(clientId)) {
                            Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} {nameof(WsMessage)}.{nameof(WsMessage.Drives)}");
                        }
                        AppRoot.OperationMqSender.SendDrives(session.LoginName, clientId, drives);
                    }
                },
                [WsMessage.LocalIps] = (session, clientId, message) => {
                    if (message.TryGetData(out List<LocalIpDto> localIps)) {
                        if (ServerRoot.IsMinerClientTestId(clientId)) {
                            Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} {nameof(WsMessage)}.{nameof(WsMessage.LocalIps)}");
                        }
                        AppRoot.OperationMqSender.SendLocalIps(session.LoginName, clientId, localIps);
                    }
                },
                [WsMessage.OperationReceived] = (session, clientId, message) => {
                    if (ServerRoot.IsMinerClientTestId(clientId)) {
                        Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} {nameof(WsMessage)}.{nameof(WsMessage.OperationReceived)}");
                    }
                    AppRoot.OperationMqSender.SendOperationReceived(session.LoginName, clientId);
                },
                [WsMessage.Speed] = (session, clientId, message) => {
                    if (message.TryGetData(out SpeedDto speedDto)) {
                        if (ServerRoot.IsMinerClientTestId(clientId)) {
                            Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} {nameof(WsMessage)}.{nameof(WsMessage.Speed)}");
                        }
                        AppRoot.SpeedDataRedis.SetAsync(new SpeedData(speedDto, DateTime.Now)).ContinueWith(t => {
                            MqBufferRoot.SendSpeed(new ClientIdIp(speedDto.ClientId, session.RemoteEndPoint.ToString()));
                        });
                    }
                },
                [WsMessage.SelfWorkLocalJson] = (session, clientId, message) => {
                    if (message.TryGetData(out string json)) {
                        if (ServerRoot.IsMinerClientTestId(clientId)) {
                            Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} {nameof(WsMessage)}.{nameof(WsMessage.SelfWorkLocalJson)}");
                        }
                        AppRoot.OperationMqSender.SendSelfWorkLocalJson(session.LoginName, clientId, json);
                    }
                },
                [WsMessage.GpuProfilesJson] = (session, clientId, message) => {
                    if (message.TryGetData(out string json)) {
                        if (ServerRoot.IsMinerClientTestId(clientId)) {
                            Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} {nameof(WsMessage)}.{nameof(WsMessage.GpuProfilesJson)}");
                        }
                        AppRoot.OperationMqSender.SendGpuProfilesJson(session.LoginName, clientId, json);
                    }
                }
            };

        public static bool TryGetHandler(string wsMessageType, out Action<IMinerClientSession, Guid, WsMessage> handler) {
            return _handlers.TryGetValue(wsMessageType, out handler);
        }
    }
}
