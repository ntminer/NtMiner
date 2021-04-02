using NTMiner.Core.MinerServer;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class MinerClientMqMessagePath : AbstractMqMessagePath {
        private readonly string _queryClientsForWsResponseRoutingKey;

        public override bool IsReadyToBuild {
            get { return true; }
        }

        public MinerClientMqMessagePath(string queue, string thisServerAddress) : base(queue) {
            _queryClientsForWsResponseRoutingKey = string.Format(MqKeyword.QueryClientsForWsResponseRoutingKey, thisServerAddress);
        }

        protected override Dictionary<string, Action<BasicDeliverEventArgs>> GetPaths() {
            return new Dictionary<string, Action<BasicDeliverEventArgs>> {
                [_queryClientsForWsResponseRoutingKey] = ea => {
                    string appId = ea.BasicProperties.AppId;
                    string mqCorrelationId = ea.BasicProperties.CorrelationId;
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string sessionId = ea.BasicProperties.ReadHeaderString(MqKeyword.SessionIdHeaderName);
                    QueryClientsResponse response = MinerClientMqBodyUtil.GetQueryClientsResponseMqReceiveBody(ea.Body);
                    if (response != null) {
                        VirtualRoot.RaiseEvent(new QueryClientsForWsResponseMqEvent(appId, mqCorrelationId, ea.GetTimestamp(), loginName, sessionId, response));
                    }
                }
            };
        }
    }
}
