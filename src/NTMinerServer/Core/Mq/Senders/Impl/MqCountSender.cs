using NTMiner.ServerNode;
using RabbitMQ.Client;

namespace NTMiner.Core.Mq.Senders.Impl {
    public class MqCountSender : IMqCountSender {
        private readonly IMq _mq;
        public MqCountSender(IMq mq) {
            _mq = mq;
        }

        public void SendMqCounts(MqCountData data) {
            if (data == null) {
                return;
            }
            _mq.BasicPublish(
                routingKey: MqKeyword.MqCountRoutingKey,
                basicProperties: CreateBasicProperties(),
                body: MqCountMqBodyUtil.GetMqCountMqSendBody(data));
        }

        private IBasicProperties CreateBasicProperties() {
            var basicProperties = _mq.CreateBasicProperties();
            basicProperties.Persistent = false;
            basicProperties.Expiration = MqKeyword.Expiration36sec;

            return basicProperties;
        }
    }
}
