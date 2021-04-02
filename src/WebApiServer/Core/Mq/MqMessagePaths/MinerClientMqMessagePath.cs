using NTMiner.Core.MinerServer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class MinerClientMqMessagePath : AbstractMqMessagePath<ClientSetInitedEvent> {
        public MinerClientMqMessagePath(string queue) : base(queue) {
        }

        protected override void Build(IModel channal) {
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.SpeedsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerSignSetedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.QueryClientsForWsRoutingKey, arguments: null);

            NTMinerConsole.UserOk("MinerClientMq QueueBind成功");
        }

        public override bool Go(BasicDeliverEventArgs ea) {
            switch (ea.RoutingKey) {
                case MqKeyword.SpeedsRoutingKey: {
                        ClientIdIp[] clientIdIps = MinerClientMqBodyUtil.GetClientIdIpsMqReciveBody(ea.Body);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new SpeedDatasMqEvent(appId, clientIdIps, ea.GetTimestamp()));
                    }
                    break;
                case MqKeyword.MinerSignSetedRoutingKey: {
                        string appId = ea.BasicProperties.AppId;
                        MinerSign minerSign = MinerClientMqBodyUtil.GetMinerSignMqReceiveBody(ea.Body);
                        if (minerSign != null) {
                            VirtualRoot.RaiseEvent(new MinerSignSetedMqEvent(appId, minerSign, ea.GetTimestamp()));
                        }
                    }
                    break;
                case MqKeyword.QueryClientsForWsRoutingKey: {
                        string appId = ea.BasicProperties.AppId;
                        string mqMessageId = ea.BasicProperties.MessageId;
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        string sessionId = ea.BasicProperties.ReadHeaderString(MqKeyword.SessionIdHeaderName);
                        QueryClientsForWsRequest query = MinerClientMqBodyUtil.GetQueryClientsForWsMqReceiveBody(ea.Body);
                        if (query != null) {
                            VirtualRoot.Execute(new QueryClientsForWsMqCommand(appId, mqMessageId, ea.GetTimestamp(), loginName, sessionId, query));
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
