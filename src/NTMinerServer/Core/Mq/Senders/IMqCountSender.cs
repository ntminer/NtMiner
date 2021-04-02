using NTMiner.ServerNode;

namespace NTMiner.Core.Mq.Senders {
    public interface IMqCountSender : IMqSender {
        void SendMqCounts(MqCountData data);
    }
}
