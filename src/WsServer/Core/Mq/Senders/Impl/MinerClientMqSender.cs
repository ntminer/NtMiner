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

        public void SendSpeed(string loginName, Guid clientId, string minerIp) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty || string.IsNullOrEmpty(minerIp)) {
                return;
            }
            var basicProperties = CreateNonePersistentBasicProperties(loginName);
            basicProperties.Headers[MqKeyword.MinerIpHeaderName] = minerIp;
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.SpeedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetClientIdMqSendBody(clientId));
        }

        public void SendMinerClientWsOpened(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            var basicProperties = CreateNonePersistentBasicProperties(loginName);
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerClientWsOpenedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetClientIdMqSendBody(clientId));
        }

        public void SendMinerClientWsClosed(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            var basicProperties = CreateNonePersistentBasicProperties(loginName);
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerClientWsClosedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetClientIdMqSendBody(clientId));
        }

        public void SendMinerClientWsBreathed(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            var basicProperties = CreateNonePersistentBasicProperties(loginName);
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerClientWsBreathedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetClientIdMqSendBody(clientId));
        }

        public void SendChangeMinerSign(MinerSign minerSign) {
            if (minerSign == null || string.IsNullOrEmpty(minerSign.Id)) {
                return;
            }
            var basicProperties = CreateBasicProperties();
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.ChangeMinerSignRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetChangeMinerSignMqSendBody(minerSign));
        }

        public void SendQueryClientsForWs(string sessionId, QueryClientsForWsRequest request) {
            if (string.IsNullOrEmpty(sessionId) || request == null || string.IsNullOrEmpty(request.LoginName)) {
                return;
            }
            var basicProperties = CreateWsBasicProperties(request.LoginName, sessionId);
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.QueryClientsForWsRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetQueryClientsForWsMqSendBody(request));
        }

        private IBasicProperties CreateWsBasicProperties(string loginName, string sessionId) {
            var basicProperties = _mq.MqChannel.CreateBasicProperties();
            basicProperties.Persistent = false;// 非持久化的
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.AppId = ServerRoot.HostConfig.ThisServerAddress;
            basicProperties.Expiration = MqKeyword.Expiration36sec;
            basicProperties.Headers = new Dictionary<string, object> {
                [MqKeyword.LoginNameHeaderName] = loginName,
                [MqKeyword.SessionIdHeaderName] = sessionId
            };

            return basicProperties;
        }

        private IBasicProperties CreateNonePersistentBasicProperties(string loginName) {
            var basicProperties = _mq.MqChannel.CreateBasicProperties();
            basicProperties.Persistent = false;// 非持久化的
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.AppId = ServerRoot.HostConfig.ThisServerAddress;
            basicProperties.Expiration = MqKeyword.Expiration36sec;
            basicProperties.Headers = new Dictionary<string, object> {
                [MqKeyword.LoginNameHeaderName] = loginName
            };

            return basicProperties;
        }

        private IBasicProperties CreateBasicProperties() {
            var basicProperties = _mq.MqChannel.CreateBasicProperties();
            basicProperties.Persistent = true;// 持久化的
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.AppId = ServerRoot.HostConfig.ThisServerAddress;

            return basicProperties;
        }
    }
}
