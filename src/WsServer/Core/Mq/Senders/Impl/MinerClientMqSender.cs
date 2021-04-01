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

        public void SendSpeeds(ClientIdIp[] clientIdIps) {
            if (clientIdIps == null || clientIdIps.Length == 0) {
                return;
            }
            var basicProperties = CreateNonePersistentBasicProperties();
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.SpeedsRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetClientIdIpsMqSendBody(clientIdIps));
        }

        public void SendMinerClientWsOpened(Guid clientId) {
            if (clientId == Guid.Empty) {
                return;
            }
            var basicProperties = CreateNonePersistentBasicProperties();
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerClientWsOpenedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetClientIdMqSendBody(clientId));
        }

        public void SendMinerClientWsClosed(Guid clientId) {
            if (clientId == Guid.Empty) {
                return;
            }
            var basicProperties = CreateNonePersistentBasicProperties();
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerClientWsClosedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetClientIdMqSendBody(clientId));
        }

        public void SendMinerClientsWsBreathed(Guid[] clientIds) {
            if (clientIds == null || clientIds.Length == 0) {
                return;
            }
            var basicProperties = CreateNonePersistentBasicProperties();
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerClientsWsBreathedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetClientIdsMqSendBody(clientIds));
        }

        public void SendMinerSignSeted(MinerSign minerSign) {
            if (minerSign == null || string.IsNullOrEmpty(minerSign.Id)) {
                return;
            }
            var basicProperties = CreatePersistentBasicProperties(minerSign.LoginName);
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerSignSetedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetMinerSignMqSendBody(minerSign));
        }

        public void SendQueryClientsForWs(string sessionId, QueryClientsForWsRequest request) {
            if (string.IsNullOrEmpty(sessionId) || request == null || string.IsNullOrEmpty(request.LoginName)) {
                return;
            }
            var basicProperties = CreateNonePersistentWsBasicProperties(request.LoginName, sessionId);
            basicProperties.Priority = 9;
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.QueryClientsForWsRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetQueryClientsForWsMqSendBody(request));
        }

        private IBasicProperties CreateNonePersistentWsBasicProperties(string loginName, string sessionId) {
            var basicProperties = _mq.CreateBasicProperties();
            basicProperties.Persistent = false;// 非持久化的
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.Expiration = MqKeyword.Expiration36sec;
            basicProperties.Headers = new Dictionary<string, object> {
                [MqKeyword.LoginNameHeaderName] = loginName,
                [MqKeyword.SessionIdHeaderName] = sessionId
            };

            return basicProperties;
        }

        private IBasicProperties CreateNonePersistentBasicProperties() {
            var basicProperties = _mq.CreateBasicProperties();
            basicProperties.Persistent = false;// 非持久化的
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.Expiration = MqKeyword.Expiration36sec;

            return basicProperties;
        }

        private IBasicProperties CreateNonePersistentBasicProperties(string loginName) {
            var basicProperties = _mq.CreateBasicProperties();
            basicProperties.Persistent = false;// 非持久化的
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.Expiration = MqKeyword.Expiration36sec;
            basicProperties.Headers = new Dictionary<string, object> {
                [MqKeyword.LoginNameHeaderName] = loginName
            };

            return basicProperties;
        }

        private IBasicProperties CreatePersistentBasicProperties(string loginName) {
            var basicProperties = _mq.CreateBasicProperties();
            basicProperties.Persistent = true;// 持久化的
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.Headers = new Dictionary<string, object> {
                [MqKeyword.LoginNameHeaderName] = loginName
            };

            return basicProperties;
        }
    }
}
