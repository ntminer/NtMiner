using NTMiner.Core;
using NTMiner.Core.Mq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public static class MqBufferRoot {
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

        static MqBufferRoot() {
            // 这样做以消减WebApiServer收到的Mq消息的数量，能消减90%以上，降低CPU使用率
            // 还可以继续消减，将这每秒钟6个Mq消息消减到1个，但是感觉没有什么必要了。
            VirtualRoot.BuildEventPath<Per1SecondEvent>("每1秒钟将暂存的数据发送到Mq", LogEnum.None, message => {
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
            }, typeof(MqBufferRoot));
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
    }
}
