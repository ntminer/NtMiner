using NTMiner.Cryptography;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.Senders.Impl {
    public class UserMqSender : IUserMqSender {
        private readonly IModel _mqChannel;
        public UserMqSender(IModel mqChannel) {
            _mqChannel = mqChannel;
        }

        public void SendUpdateUserRSAKey(string loginName, RSAKey key) {
            if (string.IsNullOrEmpty(loginName) || key == null) {
                return;
            }
            _mqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.UpdateUserRSAKeyRoutingKey,
                basicProperties: CreateBasicProperties(loginName),
                body: UserMqBodyUtil.GetUpdateUserRSAKeyMqSendBody(key));
        }

        private IBasicProperties CreateBasicProperties(string loginName) {
            var basicProperties = _mqChannel.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.AppId = ServerRoot.HostConfig.ThisServerAddress;
            basicProperties.Headers = new Dictionary<string, object> {
                [MqKeyword.LoginNameHeaderName] = loginName
            };

            return basicProperties;
        }
    }
}
