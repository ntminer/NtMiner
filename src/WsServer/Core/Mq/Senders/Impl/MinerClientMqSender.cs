using NTMiner.Core.MinerServer;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.Senders.Impl {
    public class MinerClientMqSender : IMinerClientMqSender {
        private readonly IModel _mqChannel;
        public MinerClientMqSender(IModel mqChannel) {
            _mqChannel = mqChannel;
        }

        public void SendSpeed(string loginName, Guid clientId, string minerIp) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty || string.IsNullOrEmpty(minerIp)) {
                return;
            }
            var basicProperties = CreateBasicProperties(loginName);
            basicProperties.Headers[MqKeyword.MinerIpHeaderName] = minerIp;
            _mqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.SpeedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetClientIdMqSendBody(clientId));
        }

        public void SendMinerClientWsOpened(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            var basicProperties = CreateBasicProperties(loginName);
            _mqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerClientWsOpenedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetClientIdMqSendBody(clientId));
        }

        public void SendMinerClientWsClosed(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            var basicProperties = CreateBasicProperties(loginName);
            _mqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.MinerClientWsClosedRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetClientIdMqSendBody(clientId));
        }

        public void SendMinerClientWsBreathed(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            var basicProperties = CreateBasicProperties(loginName);
            _mqChannel.BasicPublish(
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
            _mqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.ChangeMinerSignRoutingKey,
                basicProperties: basicProperties,
                body: MinerClientMqBodyUtil.GetChangeMinerSignMqSendBody(minerSign));
        }

        private IBasicProperties CreateBasicProperties(string loginName) {
            var basicProperties = _mqChannel.CreateBasicProperties();
            basicProperties.Persistent = false;// 非持久化的
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.AppId = ServerRoot.HostConfig.ThisServerAddress;
            basicProperties.Expiration = "36000000"; // 36秒，单位是微秒（1微秒是10的负6次方秒）
            basicProperties.Headers = new Dictionary<string, object> {
                [MqKeyword.LoginNameHeaderName] = loginName
            };

            return basicProperties;
        }

        private IBasicProperties CreateBasicProperties() {
            var basicProperties = _mqChannel.CreateBasicProperties();
            basicProperties.Persistent = true;// 持久化的
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.AppId = ServerRoot.HostConfig.ThisServerAddress;

            return basicProperties;
        }
    }
}
