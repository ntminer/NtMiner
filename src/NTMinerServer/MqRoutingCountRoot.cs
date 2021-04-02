using NTMiner.ServerNode;
using System.Collections.Generic;

namespace NTMiner {
    public static class MqRoutingCountRoot {
        private static readonly Dictionary<string, MqReceivedCountData> _receivedCounts = new Dictionary<string, MqReceivedCountData>();
        private static readonly object _lockerForReceivedCounts = new object();
        private static readonly Dictionary<string, MqSendCountData> _sendCounts = new Dictionary<string, MqSendCountData>();
        private static readonly object _lockerForSendCounts = new object();
        public static IEnumerable<MqReceivedCountData> RoutingCounts {
            get { return _receivedCounts.Values; }
        }

        public static void ReceivedCount(string routingKey, string queue) {
            string key = $"{routingKey}->{queue}";
            lock (_lockerForReceivedCounts) {
                if (_receivedCounts.TryGetValue(key, out MqReceivedCountData count)) {
                    if (count.Count == long.MaxValue) {
                        count.Count = 0;
                    }
                    count.Count += 1;
                }
                else {
                    _receivedCounts[key] = new MqReceivedCountData {
                        RoutingKey = routingKey,
                        Queue = queue,
                        Count = 1
                    };
                }
            }
        }

        public static void SendCount(string routingKey) {
            lock (_lockerForSendCounts) {
                if (_sendCounts.TryGetValue(routingKey, out MqSendCountData count)) {
                    if (count.Count == long.MaxValue) {
                        count.Count = 0;
                    }
                    count.Count += 1;
                }
                else {
                    _sendCounts[routingKey] = new MqSendCountData {
                        RoutingKey = routingKey,
                        Count = 1
                    };
                }
            }
        }
    }
}
