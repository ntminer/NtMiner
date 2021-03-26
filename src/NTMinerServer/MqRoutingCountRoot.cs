using NTMiner.ServerNode;
using System.Collections.Generic;

namespace NTMiner {
    public static class MqRoutingCountRoot {
        private static readonly Dictionary<string, MqRoutingCountData> _routingCounts = new Dictionary<string, MqRoutingCountData>();
        public static IEnumerable<MqRoutingCountData> RoutingCounts {
            get { return _routingCounts.Values; }
        }

        public static void Count(string routingKey, string queue) {
            if (_routingCounts.TryGetValue(routingKey, out MqRoutingCountData count)) {
                if (count.Count == long.MaxValue) {
                    count.Count = 0;
                }
                count.Count += 1;
            }
            else {
                _routingCounts[routingKey] = new MqRoutingCountData {
                    RoutingKey = routingKey,
                    Queue = queue,
                    Count = 1
                };
            }
        }

    }
}
