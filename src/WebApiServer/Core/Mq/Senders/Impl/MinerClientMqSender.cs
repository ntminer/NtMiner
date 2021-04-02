using NTMiner.Core.MinerServer;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.Senders.Impl {
    public class MinerClientMqSender : IMinerClientMqSender {
        private readonly IMq _mq;
        public MinerClientMqSender(IMq mq) {
            _mq = mq;
        }

        public void SendMinerDataRemoved(string minerId, Guid clientId) {
            if (string.IsNullOrEmpty(minerId)) {
                return;
            }
            var basicProperties = CreateBasicProperties(clientId);
            _mq.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerDataRemovedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetMinerIdMqSendBody(minerId));
        }

        public void SendResponseClientsForWs(
            string wsServerIp, 
            string loginName, 
            string sessionId, 
            string mqCorrelationId, 
            QueryClientsResponse response) {
            if (response == null) {
                return;
            }
            var basicProperties = CreateWsBasicProperties(loginName, sessionId);
            basicProperties.Priority = 9;
            if (!string.IsNullOrEmpty(mqCorrelationId)) {
                basicProperties.CorrelationId = mqCorrelationId;
            }
            _mq.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: string.Format(MqKeyword.QueryClientsForWsResponseRoutingKey, wsServerIp),
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetQueryClientsResponseMqSendBody(response));
        }

        private IBasicProperties CreateWsBasicProperties(string loginName, string sessionId) {
            var basicProperties = _mq.CreateBasicProperties();
            basicProperties.Persistent = false;// 非持久化的
            basicProperties.Expiration = MqKeyword.Expiration36sec;
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.Headers = new Dictionary<string, object> {
                [MqKeyword.LoginNameHeaderName] = loginName,
                [MqKeyword.SessionIdHeaderName] = sessionId
            };

            return basicProperties;
        }

        private IBasicProperties CreateBasicProperties(Guid clientId) {
            var basicProperties = _mq.CreateBasicProperties();
            basicProperties.Persistent = true;// 持久化的
            basicProperties.Expiration = MqKeyword.Expiration60sec;
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.Headers = new Dictionary<string, object> {
                [MqKeyword.ClientIdHeaderName] = clientId.ToString()
            };

            return basicProperties;
        }
    }
}
