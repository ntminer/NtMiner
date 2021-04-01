using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class WsBreathMqMessagePath : AbstractMqMessagePath<ClientSetInitedEvent> {
        public WsBreathMqMessagePath(string queue) : base(queue) {
        }

        protected override void Build(IModel channal) {
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerClientWsClosedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerClientsWsBreathedRoutingKey, arguments: null);

            NTMinerConsole.UserOk("WsBreathedMq QueueBind成功");
        }

        public override bool Go(BasicDeliverEventArgs ea) {
            switch (ea.RoutingKey) {
                case MqKeyword.MinerClientWsClosedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        Guid clientId = MinerClientMqBodyUtil.GetClientIdMqReciveBody(ea.Body);
                        if (clientId != Guid.Empty) {
                            VirtualRoot.RaiseEvent(new MinerClientWsClosedMqEvent(appId, clientId, timestamp));
                        }
                    }
                    break;
                case MqKeyword.MinerClientsWsBreathedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        Guid[] clientIds = MinerClientMqBodyUtil.GetClientIdsMqReciveBody(ea.Body);
                        if (clientIds != null && clientIds.Length != 0) {
                            VirtualRoot.RaiseEvent(new MinerClientsWsBreathedMqEvent(appId, clientIds, timestamp));
                        }
                    }
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}
