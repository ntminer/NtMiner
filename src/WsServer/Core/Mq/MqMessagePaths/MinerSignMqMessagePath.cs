using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class MinerSignMqMessagePath : AbstractMqMessagePath<MinerSignSetInitedEvent, UserSetInitedEvent> {
        public MinerSignMqMessagePath(string queue) : base(queue) {
        }

        protected override void Build(IModel channal) {
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerDataAddedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerDataRemovedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerSignChangedRoutingKey, arguments: null);

            NTMinerConsole.UserOk("MinerSignMq QueueBind成功");
        }

        public override void Go(BasicDeliverEventArgs ea) {
            switch (ea.RoutingKey) {
                case MqKeyword.MinerDataAddedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        string minerId = MinerClientMqBodyUtil.GetMinerIdMqReciveBody(ea.Body);
                        if (!string.IsNullOrEmpty(minerId)) {
                            VirtualRoot.RaiseEvent(new MinerDataAddedMqMessage(appId, minerId, timestamp));
                        }
                    }
                    break;
                case MqKeyword.MinerDataRemovedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        string minerId = MinerClientMqBodyUtil.GetMinerIdMqReciveBody(ea.Body);
                        if (!string.IsNullOrEmpty(minerId)) {
                            VirtualRoot.RaiseEvent(new MinerDataRemovedMqMessage(appId, minerId, timestamp));
                        }
                    }
                    break;
                case MqKeyword.MinerSignChangedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        string minerId = MinerClientMqBodyUtil.GetMinerIdMqReciveBody(ea.Body);
                        if (!string.IsNullOrEmpty(minerId)) {
                            VirtualRoot.RaiseEvent(new MinerSignChangedMqMessage(appId, minerId, timestamp));
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
