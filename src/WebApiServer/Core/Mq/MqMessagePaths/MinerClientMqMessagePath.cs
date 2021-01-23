using NTMiner.Core.MinerServer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class MinerClientMqMessagePath : AbstractMqMessagePath<ClientSetInitedEvent> {
        public MinerClientMqMessagePath(string queue) : base(queue) {
        }

        protected override void Build(IModel channal) {
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.SpeedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerClientWsOpenedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerClientWsClosedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerClientWsBreathedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.ChangeMinerSignRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.QueryClientsForWsRoutingKey, arguments: null);

            NTMinerConsole.UserOk("MinerClientMq QueueBind成功");
        }

        public override bool Go(BasicDeliverEventArgs ea) {
            switch (ea.RoutingKey) {
                // 上报的算力放在这里消费，因为只有WebApiServer消费该类型的消息，WsServer不消费该类型的消息
                case MqKeyword.SpeedRoutingKey: {
                        Guid clientId = MinerClientMqBodyUtil.GetClientIdMqReciveBody(ea.Body);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        string minerIp = ea.BasicProperties.ReadHeaderString(MqKeyword.MinerIpHeaderName);
                        VirtualRoot.RaiseEvent(new SpeedDataMqMessage(appId, clientId, minerIp, timestamp));
                    }
                    break;
                case MqKeyword.MinerClientWsOpenedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        Guid clientId = MinerClientMqBodyUtil.GetClientIdMqReciveBody(ea.Body);
                        if (clientId != Guid.Empty) {
                            VirtualRoot.RaiseEvent(new MinerClientWsOpenedMqMessage(appId, clientId, timestamp));
                        }
                    }
                    break;
                case MqKeyword.MinerClientWsClosedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        Guid clientId = MinerClientMqBodyUtil.GetClientIdMqReciveBody(ea.Body);
                        if (clientId != Guid.Empty) {
                            VirtualRoot.RaiseEvent(new MinerClientWsClosedMqMessage(appId, clientId, timestamp));
                        }
                    }
                    break;
                case MqKeyword.MinerClientWsBreathedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        Guid clientId = MinerClientMqBodyUtil.GetClientIdMqReciveBody(ea.Body);
                        if (clientId != Guid.Empty) {
                            VirtualRoot.RaiseEvent(new MinerClientWsBreathedMqMessage(appId, clientId, timestamp));
                        }
                    }
                    break;
                case MqKeyword.ChangeMinerSignRoutingKey: {
                        MinerSign minerSign = MinerClientMqBodyUtil.GetChangeMinerSignMqReceiveBody(ea.Body);
                        if (minerSign != null) {
                            VirtualRoot.Execute(new ChangeMinerSignMqMessage(minerSign));
                        }
                    }
                    break;
                case MqKeyword.QueryClientsForWsRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        string sessionId = ea.BasicProperties.ReadHeaderString(MqKeyword.SessionIdHeaderName);
                        QueryClientsForWsRequest query = MinerClientMqBodyUtil.GetQueryClientsForWsMqReceiveBody(ea.Body);
                        if (query != null) {
                            VirtualRoot.Execute(new QueryClientsForWsMqMessage(appId, timestamp, loginName, sessionId, query));
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
