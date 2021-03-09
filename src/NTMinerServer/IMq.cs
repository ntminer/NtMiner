using RabbitMQ.Client;

namespace NTMiner {
    public interface IMq {
        IModel MqChannel { get; }
    }
}
