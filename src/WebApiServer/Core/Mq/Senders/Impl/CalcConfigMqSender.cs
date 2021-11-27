using RabbitMQ.Client;

namespace NTMiner.Core.Mq.Senders.Impl {
    public class CalcConfigMqSender : ICalcConfigMqSender {
        private static readonly byte[] _emptyBody = new byte[0];
        private readonly IMq _mq;
        public CalcConfigMqSender(IMq mq) {
            _mq = mq;
        }

        public void SendCalcConfigsUpdated() {
            _mq.BasicPublish(
                routingKey: MqKeyword.CalcConfigsUpdatedRoutingKey,
                basicProperties: CreateBasicProperties(),
                body: _emptyBody);
        }

        private IBasicProperties CreateBasicProperties() {
            var basicProperties = _mq.CreateBasicProperties();
            basicProperties.Persistent = false;
            basicProperties.Expiration = MqKeyword.Expiration60sec;

            return basicProperties;
        }
    }
}
