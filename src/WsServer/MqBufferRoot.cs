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

        static MqBufferRoot() {
            // 这样做以消减WebApiServer收到的Mq消息的数量，能消减90%以上，降低CPU使用率
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
            }, typeof(WsMessageFromMinerClientHandler));
        }

        public static void Breath(Guid clientId) {
            lock (_lockerForToBreathClientIds) {
                _toBreathClientIds.Add(clientId);
            }
        }

        public static void AddClientIdIp(ClientIdIp clientIdIp) {
            lock (_lockerForSpeedClientIdIps) {
                _speedClientIdIps.Add(clientIdIp);
            }
        }
    }
}
