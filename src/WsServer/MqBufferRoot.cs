using NTMiner.Core;
using NTMiner.Core.Mq;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using NTMiner.Core.MinerServer;

namespace NTMiner {
    public static class MqBufferRoot {
        private static readonly ConcurrentDictionary<Guid, DateTime> _fastIdDic = new ConcurrentDictionary<Guid, DateTime>();
        private static readonly List<Guid> _toBreathClientIds = new List<Guid>();
        private static readonly object _lockerForToBreathClientIds = new object();
        private static readonly List<ClientIdIp> _speedClientIdIps = new List<ClientIdIp>();
        private static readonly object _lockerForSpeedClientIdIps = new object();
        private static readonly List<UserGetSpeedRequest> _userGetSpeedRequests = new List<UserGetSpeedRequest>();
        private static readonly object _lockerForUserGetSpeedRequests = new object();
        private static readonly List<AfterTimeRequest> _getConsoleOutLinesRequests = new List<AfterTimeRequest>();
        private static readonly object _lockerForGetConsoleOutLinesRequests = new object();
        private static readonly List<AfterTimeRequest> _getLocalMessagesRequests = new List<AfterTimeRequest>();
        private static readonly object _lockerForGetLocalMessagesRequests = new object();
        private static readonly List<AfterTimeRequest> _getOperationResultsRequests = new List<AfterTimeRequest>();
        private static readonly object _lockerForGetOperationResultsRequests = new object();
        private static readonly List<ConsoleOutLines> _consoleOutLineses = new List<ConsoleOutLines>();
        private static readonly object _lockForConsoleOutLineses = new object();
        private static readonly List<LocalMessages> _localMessageses = new List<LocalMessages>();
        private static readonly object _lockForLocalMessageses = new object();
        private static readonly List<OperationResults> _operationResultses = new List<OperationResults>();
        private static readonly object _lockForOperationResultses = new object();
        private static readonly List<QueryClientsForWsRequest> _autoQueryClientDatas = new List<QueryClientsForWsRequest>();
        private static readonly object _lockForAutoQueryClientDatas = new object();

        static MqBufferRoot() {
            // 这样做以消减WebApiServer收到的Mq消息的数量，能消减90%以上，降低CPU使用率
            // 还可以继续消减，将这每秒钟6个Mq消息消减到1个，但是感觉没有什么必要了。
            // WsServer全是2核4G内存的windows
            VirtualRoot.BuildEventPath<Per1SecondEvent>("每1秒钟将暂存的数据发送到Mq", LogEnum.None, typeof(MqBufferRoot), PathPriority.Normal, message => {
                Task.Factory.StartNew(() => {
                    Guid[] clientIds;
                    lock (_lockerForToBreathClientIds) {
                        clientIds = _toBreathClientIds.ToArray();
                        _toBreathClientIds.Clear();
                    }
                    AppRoot.MinerClientMqSender.SendMinerClientsWsBreathed(clientIds);
                });
                Task.Factory.StartNew(() => {
                    ClientIdIp[] clientIdIps;
                    lock (_lockerForSpeedClientIdIps) {
                        clientIdIps = _speedClientIdIps.ToArray();
                        _speedClientIdIps.Clear();
                    }
                    AppRoot.MinerClientMqSender.SendSpeeds(clientIdIps);
                });
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

                    AfterTimeRequest[] getLocalMessagesRequests;
                    lock (_lockerForGetLocalMessagesRequests) {
                        getLocalMessagesRequests = _getLocalMessagesRequests.ToArray();
                        _getLocalMessagesRequests.Clear();
                    }
                    AppRoot.OperationMqSender.SendGetLocalMessages(getLocalMessagesRequests);

                    AfterTimeRequest[] getOperationResultsRequests;
                    lock (_lockerForGetOperationResultsRequests) {
                        getOperationResultsRequests = _getOperationResultsRequests.ToArray();
                        _getOperationResultsRequests.Clear();
                    }
                    AppRoot.OperationMqSender.SendGetOperationResults(getOperationResultsRequests);
                });
                Task.Factory.StartNew(() => {
                    ConsoleOutLines[] consoleOutLineses;
                    lock (_lockForConsoleOutLineses) {
                        consoleOutLineses = _consoleOutLineses.ToArray();
                        _consoleOutLineses.Clear();
                    }
                    AppRoot.OperationMqSender.SendConsoleOutLineses(consoleOutLineses);

                    LocalMessages[] localMessageses;
                    lock (_lockForLocalMessageses) {
                        localMessageses = _localMessageses.ToArray();
                        _localMessageses.Clear();
                    }
                    AppRoot.OperationMqSender.SendLocalMessageses(localMessageses);

                    OperationResults[] operationResultses;
                    lock (_lockForOperationResultses) {
                        operationResultses = _operationResultses.ToArray();
                        _operationResultses.Clear();
                    }
                    AppRoot.OperationMqSender.SendOperationResultses(operationResultses);

                    QueryClientsForWsRequest[] queryClientsRequests;
                    lock (_lockForAutoQueryClientDatas) {
                        queryClientsRequests = _autoQueryClientDatas.ToArray();
                        _autoQueryClientDatas.Clear();
                    }
                    AppRoot.MinerClientMqSender.SendAutoQueryClientsForWs(queryClientsRequests);
                });
            });
            VirtualRoot.BuildEventPath<Per1MinuteEvent>("周期清理内存中过期的fastId", LogEnum.None, typeof(MqBufferRoot), PathPriority.Normal, message => {
                DateTime dt = message.BornOn.AddMinutes(-1);
                var keys = _fastIdDic.Where(a => a.Value <= dt).Select(a => a.Key).ToArray();
                foreach (var key in keys) {
                    _fastIdDic.TryRemove(key, out _);
                }
            });
        }

        public static void AddFastId(Guid messageId) {
            _fastIdDic.TryAdd(messageId, DateTime.Now);
        }

        /// <summary>
        /// 如果给定的messageId是FastId则返回true，否则返回false。
        /// </summary>
        /// <param name="messageId">给定的messageId</param>
        /// <returns></returns>
        public static bool TryRemoveFastId(Guid messageId) {
            return _fastIdDic.TryRemove(messageId, out _);
        }

        public static void Breath(Guid clientId) {
            lock (_lockerForToBreathClientIds) {
                _toBreathClientIds.Add(clientId);
            }
        }

        // SpeedData已存入redis，这里只发送clientId和clientIp
        public static void SendSpeed(ClientIdIp clientIdIp) {
            lock (_lockerForSpeedClientIdIps) {
                _speedClientIdIps.Add(clientIdIp);
            }
        }

        public static void GetConsoleOutLines(AfterTimeRequest request) {
            lock (_lockerForGetConsoleOutLinesRequests) {
                _getConsoleOutLinesRequests.Add(request);
            }
        }

        public static void GetLocalMessages(AfterTimeRequest request) {
            lock (_lockerForGetLocalMessagesRequests) {
                _getLocalMessagesRequests.Add(request);
            }
        }

        public static void GetOperationResults(AfterTimeRequest request) {
            lock (_lockerForGetOperationResultsRequests) {
                _getOperationResultsRequests.Add(request);
            }
        }

        public static void UserGetSpeed(UserGetSpeedRequest request) {
            lock (_lockerForUserGetSpeedRequests) {
                _userGetSpeedRequests.Add(request);
            }
        }

        public static void ConsoleOutLines(ConsoleOutLines consoleOutLines) {
            lock (_lockForConsoleOutLineses) {
                _consoleOutLineses.Add(consoleOutLines);
            }
        }

        public static void LocalMessages(LocalMessages localMessages) {
            lock (_lockForLocalMessageses) {
                _localMessageses.Add(localMessages);
            }
        }

        public static void OperationResults(OperationResults operationResults) {
            lock (_lockForOperationResultses) {
                _operationResultses.Add(operationResults);
            }
        }

        public static void AutoQueryClientDatas(QueryClientsForWsRequest query) {
            lock (_lockForAutoQueryClientDatas) {
                _autoQueryClientDatas.Add(query);
            }
        }
    }
}
