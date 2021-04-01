using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class MinerSignMqMessagePath : AbstractMqMessagePath<MinerSignSetInitedEvent, UserSetInitedEvent> {
        public MinerSignMqMessagePath(string queue) : base(queue) {
        }

        protected override void Build(IModel channal) {
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerDataRemovedRoutingKey, arguments: null);

            NTMinerConsole.UserOk("MinerSignMq QueueBind成功");
        }

        public override bool Go(BasicDeliverEventArgs ea) {
            switch (ea.RoutingKey) {
                case MqKeyword.MinerDataRemovedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        string minerId = MinerClientMqBodyUtil.GetMinerIdMqReciveBody(ea.Body);
                        if (!string.IsNullOrEmpty(minerId) && ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new MinerDataRemovedMqEvent(appId, minerId, clientId, timestamp));
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
