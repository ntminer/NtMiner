using NTMiner.Core.MinerServer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class MinerClientMqMessagePath : AbstractMqMessagePath<ClientSetInitedEvent> {
        public MinerClientMqMessagePath(string queue) : base(queue) {
        }

        protected override void Build(IModel channal) {
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.SpeedsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerSignChangedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.QueryClientsForWsRoutingKey, arguments: null);

            NTMinerConsole.UserOk("MinerClientMq QueueBind成功");
        }

        public override bool Go(BasicDeliverEventArgs ea) {
            switch (ea.RoutingKey) {
                case MqKeyword.SpeedsRoutingKey: {
                        ClientIdIp[] clientIdIps = MinerClientMqBodyUtil.GetClientIdIpsMqReciveBody(ea.Body);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new SpeedDatasMqEvent(appId, clientIdIps, timestamp));
                    }
                    break;
                case MqKeyword.MinerSignChangedRoutingKey: {
                        string appId = ea.BasicProperties.AppId;
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        MinerSign minerSign = MinerClientMqBodyUtil.GetMinerSignChangedMqReceiveBody(ea.Body);
                        if (minerSign != null) {
                            VirtualRoot.RaiseEvent(new MinerSignChangedMqEvent(appId, minerSign, timestamp));
                        }
                    }
                    break;
                case MqKeyword.QueryClientsForWsRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        string mqMessageId = ea.BasicProperties.MessageId;
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        string sessionId = ea.BasicProperties.ReadHeaderString(MqKeyword.SessionIdHeaderName);
                        QueryClientsForWsRequest query = MinerClientMqBodyUtil.GetQueryClientsForWsMqReceiveBody(ea.Body);
                        if (query != null) {
                            VirtualRoot.Execute(new QueryClientsForWsMqCommand(appId, mqMessageId, timestamp, loginName, sessionId, query));
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
