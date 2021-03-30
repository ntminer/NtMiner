using NTMiner.ServerNode;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.Senders {
    public interface IMqRoutingCountSender : IMqSender {
        void SendMqRoutingCounts(List<MqRoutingCountData> data);
    }
}
