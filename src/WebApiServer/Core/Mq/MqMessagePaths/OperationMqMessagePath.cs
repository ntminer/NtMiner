using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class OperationMqMessagePath : AbstractMqMessagePath {
        public OperationMqMessagePath(string queue) : base(queue) {
        }

        public override bool IsReadyToBuild {
            get {
                return true;
            }
        }

        protected override void Build(IModel channal) {
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.StartMineRoutingKey, arguments: null);
        }

        public override bool Go(BasicDeliverEventArgs ea) {
            switch (ea.RoutingKey) {
                case MqKeyword.StartMineRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            Guid workId = OperationMqBodyUtil.GetStartMineMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new StartMineMqEvent(appId, loginName, ea.GetTimestamp(), clientId, workId));
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
