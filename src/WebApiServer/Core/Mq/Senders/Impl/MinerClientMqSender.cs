using RabbitMQ.Client;

namespace NTMiner.Core.Mq.Senders.Impl {
    public class MinerClientMqSender : IMinerClientMqSender {
        private readonly IServerConnection _serverConnection;
        public MinerClientMqSender(IServerConnection serverConnection) {
            _serverConnection = serverConnection;
        }

        public void SendMinerDataAdded(string minerId) {
            if (string.IsNullOrEmpty(minerId)) {
                return;
            }
            var basicProperties = CreateBasicProperties();
            _serverConnection.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerDataAddedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetMinerIdMqSendBody(minerId));
        }

        public void SendMinerDataRemoved(string minerId) {
            if (string.IsNullOrEmpty(minerId)) {
                return;
            }
            var basicProperties = CreateBasicProperties();
            _serverConnection.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerDataRemovedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetMinerIdMqSendBody(minerId));
        }

        public void SendMinerSignChanged(string minerId) {
            if (string.IsNullOrEmpty(minerId)) {
                return;
            }
            var basicProperties = CreateBasicProperties();
            _serverConnection.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerSignChangedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetMinerIdMqSendBody(minerId));
        }

        private IBasicProperties CreateBasicProperties() {
            var basicProperties = _serverConnection.MqChannel.CreateBasicProperties();
            basicProperties.Persistent = true;// 持久化的
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.AppId = ServerRoot.HostConfig.ThisServerAddress;

            return basicProperties;
        }
    }
}
