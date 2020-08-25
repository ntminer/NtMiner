using RabbitMQ.Client.Events;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public interface IMqMessagePath {
        string Queue { get; }
        bool Go(BasicDeliverEventArgs ea);
    }
}
