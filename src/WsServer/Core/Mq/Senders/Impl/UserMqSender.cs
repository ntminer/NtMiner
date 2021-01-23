using NTMiner.Cryptography;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.Senders.Impl {
    public class UserMqSender : IUserMqSender {
        private readonly IMqRedis _mq;
        public UserMqSender(IMqRedis mq) {
            _mq = mq;
        }

        public void SendUpdateUserRSAKey(string loginName, RSAKey key) {
            if (string.IsNullOrEmpty(loginName) || key == null) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.UpdateUserRSAKeyRoutingKey,
                basicProperties: CreateBasicProperties(loginName),
                body: UserMqBodyUtil.GetUpdateUserRSAKeyMqSendBody(key));
        }

        private IBasicProperties CreateBasicProperties(string loginName) {
            var basicProperties = _mq.MqChannel.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Expiration = MqKeyword.Expiration60sec;
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.AppId = ServerRoot.HostConfig.ThisServerAddress;
            basicProperties.Headers = new Dictionary<string, object> {
                [MqKeyword.LoginNameHeaderName] = loginName
            };

            return basicProperties;
        }
    }
}
