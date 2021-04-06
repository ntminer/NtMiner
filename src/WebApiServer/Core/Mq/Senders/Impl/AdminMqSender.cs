using RabbitMQ.Client;

namespace NTMiner.Core.Mq.Senders.Impl {
    public class AdminMqSender : IAdminMqSender {
        private static readonly byte[] _emptyBody = new byte[0];
        private readonly IMq _mq;
        public AdminMqSender(IMq mq) {
            _mq = mq;
        }

        public void SendRefreshMinerTestId() {
            var basicProperties = CreateBasicProperties();
            _mq.BasicPublish(
                routingKey: MqKeyword.RefreshMinerTestIdRoutingKey,
                basicProperties: basicProperties,
                body: _emptyBody);
        }

        private IBasicProperties CreateBasicProperties() {
            var basicProperties = _mq.CreateBasicProperties();
            basicProperties.Persistent = false;// 非持久化的
            basicProperties.Expiration = MqKeyword.Expiration60sec;

            return basicProperties;
        }
    }
}
