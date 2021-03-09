using NTMiner.Core.MinerServer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class MinerClientMqMessagePath : AbstractMqMessagePath {
        private readonly string _queryClientsForWsResponseRoutingKey;

        public override bool IsReadyToBuild {
            get { return true; }
        }

        public MinerClientMqMessagePath(string queue, string thisServerAddress) : base(queue) {
            _queryClientsForWsResponseRoutingKey = string.Format(MqKeyword.QueryClientsForWsResponseRoutingKey, thisServerAddress);
        }

        protected override void Build(IModel channal) {
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: _queryClientsForWsResponseRoutingKey, arguments: null);

            NTMinerConsole.UserOk("MinerClientMq QueueBind成功");
        }

        public override bool Go(BasicDeliverEventArgs ea) {
            if (ea.RoutingKey == _queryClientsForWsResponseRoutingKey) {
                DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                string appId = ea.BasicProperties.AppId;
                string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                string sessionId = ea.BasicProperties.ReadHeaderString(MqKeyword.SessionIdHeaderName);
                QueryClientsResponse response = MinerClientMqBodyUtil.GetQueryClientsResponseMqReceiveBody(ea.Body);
                if (response != null) {
                    VirtualRoot.RaiseEvent(new QueryClientsForWsResponseMqEvent(appId, timestamp, loginName, sessionId, response));
                }
                return true;
            }
            else {
                return false;
            }
        }
    }
}
