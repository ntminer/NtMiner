using RabbitMQ.Client;

namespace NTMiner.Core.Mq.Senders.Impl {
    public class WsServerNodeMqSender : IWsServerNodeMqSender {
        private readonly IMqRedis _serverConnection;
        public WsServerNodeMqSender(IMqRedis serverConnection) {
            _serverConnection = serverConnection;
        }

        public void SendWsServerNodeAdded(string wsServerNodeAddress) {
            if (string.IsNullOrEmpty(wsServerNodeAddress)) {
                return;
            }
            _serverConnection.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange, 
                routingKey: MqKeyword.WsServerNodeAddedRoutingKey, 
                basicProperties: CreateBasicProperties(), 
                body: WsServerNodeMqBodyUtil.GetWsServerNodeAddressMqSendBody(wsServerNodeAddress));
        }

        public void SendWsServerNodeRemoved(string wsServerNodeAddress) {
            if (string.IsNullOrEmpty(wsServerNodeAddress)) {
                return;
            }
            _serverConnection.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange, 
                routingKey: MqKeyword.WsServerNodeRemovedRoutingKey,
                basicProperties: CreateBasicProperties(), 
                body: WsServerNodeMqBodyUtil.GetWsServerNodeAddressMqSendBody(wsServerNodeAddress));
        }

        private IBasicProperties CreateBasicProperties() {
            var basicProperties = _serverConnection.MqChannel.CreateBasicProperties();
            basicProperties.Persistent = false;
            basicProperties.Expiration = MqKeyword.Expiration36sec;
            basicProperties.AppId = ServerRoot.HostConfig.ThisServerAddress;

            return basicProperties;
        }
    }
}
