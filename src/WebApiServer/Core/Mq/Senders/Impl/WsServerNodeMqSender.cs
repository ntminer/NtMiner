using RabbitMQ.Client;

namespace NTMiner.Core.Mq.Senders.Impl {
    public class WsServerNodeMqSender : IWsServerNodeMqSender {
        private readonly IServerConnection _serverConnection;
        public WsServerNodeMqSender(IServerConnection serverConnection) {
            _serverConnection = serverConnection;
        }

        public void SendWsServerNodeAdded(string wsServerNodeAddress) {
            if (string.IsNullOrEmpty(wsServerNodeAddress)) {
                return;
            }
            _serverConnection.Channel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange, 
                routingKey: MqKeyword.WsServerNodeAddedRoutingKey, 
                basicProperties: CreateBasicProperties(), 
                body: WsServerNodeMqBodyUtil.GetWsServerNodeAddressMqSendBody(wsServerNodeAddress));
        }

        public void SendWsServerNodeRemoved(string wsServerNodeAddress) {
            if (string.IsNullOrEmpty(wsServerNodeAddress)) {
                return;
            }
            _serverConnection.Channel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange, 
                routingKey: MqKeyword.WsServerNodeRemovedRoutingKey,
                basicProperties: CreateBasicProperties(), 
                body: WsServerNodeMqBodyUtil.GetWsServerNodeAddressMqSendBody(wsServerNodeAddress));
        }

        private IBasicProperties CreateBasicProperties() {
            var basicProperties = _serverConnection.Channel.CreateBasicProperties();
            basicProperties.Persistent = false;
            basicProperties.Expiration = "36000000"; // 36秒，单位是微秒（1微秒是10的负6次方秒）
            basicProperties.AppId = ServerRoot.HostConfig.ThisServerAddress;

            return basicProperties;
        }
    }
}
