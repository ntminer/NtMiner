using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class WsBreathMqMessagePath : AbstractMqMessagePath<ClientSetInitedEvent> {
        public WsBreathMqMessagePath(string queue) : base(queue) {
        }

        protected override void Build(IModel channal) {
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerClientWsOpenedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerClientWsClosedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerClientWsBreathedRoutingKey, arguments: null);

            NTMinerConsole.UserOk("WsBreathedMq QueueBind成功");
        }

        public override bool Go(BasicDeliverEventArgs ea) {
            switch (ea.RoutingKey) {
                case MqKeyword.MinerClientWsOpenedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        Guid clientId = MinerClientMqBodyUtil.GetClientIdMqReciveBody(ea.Body);
                        if (clientId != Guid.Empty) {
                            VirtualRoot.RaiseEvent(new MinerClientWsOpenedMqEvent(appId, clientId, timestamp));
                        }
                    }
                    break;
                case MqKeyword.MinerClientWsClosedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        Guid clientId = MinerClientMqBodyUtil.GetClientIdMqReciveBody(ea.Body);
                        if (clientId != Guid.Empty) {
                            VirtualRoot.RaiseEvent(new MinerClientWsClosedMqEvent(appId, clientId, timestamp));
                        }
                    }
                    break;
                case MqKeyword.MinerClientWsBreathedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        Guid clientId = MinerClientMqBodyUtil.GetClientIdMqReciveBody(ea.Body);
                        if (clientId != Guid.Empty) {
                            VirtualRoot.RaiseEvent(new MinerClientWsBreathedMqEvent(appId, clientId, timestamp));
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
