using NTMiner.Core;
using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.Ws;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public static class WsMessageFromMinerStudioHandler {
        private static readonly List<UserGetSpeedRequest> _userGetSpeedRequests = new List<UserGetSpeedRequest>();
        private static readonly object _lockerForUserGetSpeedRequests = new object();
        private static readonly List<AfterTimeRequest> _getConsoleOutLinesRequests = new List<AfterTimeRequest>();
        private static readonly object _lockerForGetConsoleOutLinesRequests = new object();
        private static readonly List<AfterTimeRequest> _getLocalMessagesRequests = new List<AfterTimeRequest>();
        private static readonly object _lockerForGetLocalMessagesRequests = new object();
        private static readonly List<AfterTimeRequest> _getOperationResultsRequests = new List<AfterTimeRequest>();
        private static readonly object _lockerForGetOperationResultsRequests = new object();
        static WsMessageFromMinerStudioHandler() {
            // 这样做以消减WebApiServer收到的Mq消息的数量，能消减90%以上，降低CPU使用率
            VirtualRoot.BuildEventPath<Per1SecondEvent>("每1秒钟将WsServer暂存的来自群控客户端的消息广播到Mq", LogEnum.None, message => {
                Task.Factory.StartNew(() => {
                    UserGetSpeedRequest[] userGetSpeedRequests;
                    lock (_lockerForUserGetSpeedRequests) {
                        userGetSpeedRequests = _userGetSpeedRequests.ToArray();
                        _userGetSpeedRequests.Clear();
                    }
                    AppRoot.OperationMqSender.SendGetSpeed(userGetSpeedRequests);
                });

                Task.Factory.StartNew(() => {
                    AfterTimeRequest[] getConsoleOutLinesRequests;
                    lock (_lockerForGetConsoleOutLinesRequests) {
                        getConsoleOutLinesRequests = _getConsoleOutLinesRequests.ToArray();
                        _getConsoleOutLinesRequests.Clear();
                    }
                    AppRoot.OperationMqSender.SendGetConsoleOutLines(getConsoleOutLinesRequests);
                });

                Task.Factory.StartNew(() => {
                    AfterTimeRequest[] getLocalMessagesRequests;
                    lock (_lockerForGetLocalMessagesRequests) {
                        getLocalMessagesRequests = _getLocalMessagesRequests.ToArray();
                        _getLocalMessagesRequests.Clear();
                    }
                    AppRoot.OperationMqSender.SendGetLocalMessages(getLocalMessagesRequests);
                });

                Task.Factory.StartNew(() => {
                    AfterTimeRequest[] getOperationResultsRequests;
                    lock (_lockerForGetOperationResultsRequests) {
                        getOperationResultsRequests = _getOperationResultsRequests.ToArray();
                        _getOperationResultsRequests.Clear();
                    }
                    AppRoot.OperationMqSender.SendGetOperationResults(getOperationResultsRequests);
                });
            }, typeof(WsMessageFromMinerClientHandler));
        }

        private static readonly Dictionary<string, Action<IMinerStudioSession, WsMessage>>
            _handlers = new Dictionary<string, Action<IMinerStudioSession, WsMessage>>(StringComparer.OrdinalIgnoreCase) {
                [WsMessage.GetConsoleOutLines] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        lock (_lockerForGetConsoleOutLinesRequests) {
                            _getConsoleOutLinesRequests.Add(new AfterTimeRequest {
                                AfterTime = afterTime,
                                ClientId = wrapperClientIdData.ClientId,
                                LoginName = session.LoginName
                            });
                        }
                    }
                },
                [WsMessage.FastGetConsoleOutLines] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        AppRoot.OperationMqSender.SendFastGetConsoleOutLines(session.LoginName, wrapperClientIdData.ClientId, afterTime);
                    }
                },
                [WsMessage.GetLocalMessages] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        lock (_lockerForGetLocalMessagesRequests) {
                            _getLocalMessagesRequests.Add(new AfterTimeRequest {
                                AfterTime = afterTime,
                                ClientId = wrapperClientIdData.ClientId,
                                LoginName = session.LoginName
                            });
                        }
                    }
                },
                [WsMessage.FastGetLocalMessages] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        AppRoot.OperationMqSender.SendFastGetLocalMessages(session.LoginName, wrapperClientIdData.ClientId, afterTime);
                    }
                },
                [WsMessage.GetOperationResults] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        lock (_lockerForGetOperationResultsRequests) {
                            _getOperationResultsRequests.Add(new AfterTimeRequest {
                                AfterTime = afterTime,
                                ClientId = wrapperClientIdData.ClientId,
                                LoginName = session.LoginName
                            });
                        }
                    }
                },
                [WsMessage.FastGetOperationResults] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        AppRoot.OperationMqSender.SendFastGetOperationResults(session.LoginName, wrapperClientIdData.ClientId, afterTime);
                    }
                },
                [WsMessage.GetDrives] = (session, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        AppRoot.OperationMqSender.SendGetDrives(session.LoginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.GetLocalIps] = (session, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        AppRoot.OperationMqSender.SendGetLocalIps(session.LoginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.GetSpeed] = (session, message) => {
                    if (message.TryGetData(out List<Guid> clientIds)) {
                        lock (_lockerForUserGetSpeedRequests) {
                            _userGetSpeedRequests.Add(new UserGetSpeedRequest {
                                LoginName = session.LoginName,
                                ClientIds = clientIds
                            });
                        }
                    }
                },
                [WsMessage.EnableRemoteDesktop] = (session, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        AppRoot.OperationMqSender.SendEnableRemoteDesktop(session.LoginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.BlockWAU] = (session, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        AppRoot.OperationMqSender.SendBlockWAU(session.LoginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.SetVirtualMemory] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out Dictionary<string, int> data)) {
                        AppRoot.OperationMqSender.SendSetVirtualMemory(session.LoginName, wrapperClientIdData.ClientId, data);
                    }
                },
                [WsMessage.SetLocalIps] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<LocalIpInput> data)) {
                        AppRoot.OperationMqSender.SendSetLocalIps(session.LoginName, wrapperClientIdData.ClientId, data);
                    }
                },
                [WsMessage.SwitchRadeonGpu] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out bool on)) {
                        AppRoot.OperationMqSender.SendSwitchRadeonGpu(session.LoginName, wrapperClientIdData.ClientId, on);
                    }
                },
                [WsMessage.GetSelfWorkLocalJson] = (session, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        AppRoot.OperationMqSender.SendGetSelfWorkLocalJson(session.LoginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.SaveSelfWorkLocalJson] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out WorkRequest workRequest)) {
                        AppRoot.OperationMqSender.SendSaveSelfWorkLocalJson(session.LoginName, wrapperClientData.ClientId, workRequest);
                    }
                },
                [WsMessage.GetGpuProfilesJson] = (session, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        AppRoot.OperationMqSender.SendGetGpuProfilesJson(session.LoginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.SaveGpuProfilesJson] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out string json)) {
                        AppRoot.OperationMqSender.SendSaveGpuProfilesJson(session.LoginName, wrapperClientData.ClientId, json);
                    }
                },
                [WsMessage.SetAutoBootStart] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out SetAutoBootStartRequest body)) {
                        AppRoot.OperationMqSender.SendSetAutoBootStart(session.LoginName, wrapperClientData.ClientId, body);
                    }
                },
                [WsMessage.RestartWindows] = (session, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        AppRoot.OperationMqSender.SendRestartWindows(session.LoginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.ShutdownWindows] = (session, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        AppRoot.OperationMqSender.SendShutdownWindows(session.LoginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.UpgradeNTMiner] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out string ntminerFileName)) {
                        AppRoot.OperationMqSender.SendUpgradeNTMiner(session.LoginName, wrapperClientData.ClientId, ntminerFileName);
                    }
                },
                [WsMessage.StartMine] = (session, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out Guid workId)) {
                        AppRoot.OperationMqSender.SendStartMine(session.LoginName, wrapperClientData.ClientId, workId);
                    }
                },
                [WsMessage.StopMine] = (session, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        AppRoot.OperationMqSender.SendStopMine(session.LoginName, wrapperClientId.ClientId);
                    }
                },
                [WsMessage.QueryClientDatas] = (session, message) => {
                    if (message.TryGetData(out QueryClientsRequest query)) {
                        AppRoot.MinerClientMqSender.SendQueryClientsForWs(session.WsSessionId, QueryClientsForWsRequest.Create(query, session.LoginName));
                    }
                }
            };

        public static bool TryGetHandler(string wsMessageType, out Action<IMinerStudioSession, WsMessage> handler) {
            return _handlers.TryGetValue(wsMessageType, out handler);
        }
    }
}
