using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class WsServerNodeMqMessagePath : AbstractMqMessagePath<WsServerNodeAddressSetInitedEvent> {
        public WsServerNodeMqMessagePath(string queue) : base(queue) {
        }

        protected override void Build(IModel channal) {
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.WsServerNodeAddedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.WsServerNodeRemovedRoutingKey, arguments: null);

            Write.UserOk("WsServerNodeMq QueueBind成功");
        }

        public override void Go(BasicDeliverEventArgs ea) {
            switch (ea.RoutingKey) {
                case MqKeyword.WsServerNodeAddedRoutingKey: {
                        string wsServerAddress = WsServerNodeMqBodyUtil.GetWsServerNodeAddressMqReceiveBody(ea.Body);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new WsServerNodeAddedMqMessage(appId, wsServerAddress));
                    }
                    break;
                case MqKeyword.WsServerNodeRemovedRoutingKey: {
                        string wsServerAddress = WsServerNodeMqBodyUtil.GetWsServerNodeAddressMqReceiveBody(ea.Body);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new WsServerNodeRemovedMqMessage(appId, wsServerAddress));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
