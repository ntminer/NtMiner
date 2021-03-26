using RabbitMQ.Client;

namespace NTMiner.Core.Mq.Senders.Impl {
    public class UserMqSender : IUserMqSender {
        private readonly IMq _mq;
        public UserMqSender(IMq mq) {
            _mq = mq;
        }

        public void SendUserAdded(string loginName) {
            if (string.IsNullOrEmpty(loginName)) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange, 
                routingKey: MqKeyword.UserAddedRoutingKey, 
                basicProperties: CreateBasicProperties(), 
                body: UserMqBodyUtil.GetLoginNameMqSendBody(loginName));
        }

        public void SendUserRemoved(string loginName) {
            if (string.IsNullOrEmpty(loginName)) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.UserRemovedRoutingKey, 
                basicProperties: CreateBasicProperties(), 
                body: UserMqBodyUtil.GetLoginNameMqSendBody(loginName));
        }

        public void SendUserUpdated(string loginName) {
            if (string.IsNullOrEmpty(loginName)) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange, 
                routingKey: MqKeyword.UserUpdatedRoutingKey, 
                basicProperties: CreateBasicProperties(), 
                body: UserMqBodyUtil.GetLoginNameMqSendBody(loginName));
        }

        public void SendUserEnabled(string loginName) {
            if (string.IsNullOrEmpty(loginName)) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange, 
                routingKey: MqKeyword.UserEnabledRoutingKey, 
                basicProperties: CreateBasicProperties(), 
                body: UserMqBodyUtil.GetLoginNameMqSendBody(loginName));
        }

        public void SendUserDisabled(string loginName) {
            if (string.IsNullOrEmpty(loginName)) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange, 
                routingKey: MqKeyword.UserDisabledRoutingKey, 
                basicProperties: CreateBasicProperties(), 
                body: UserMqBodyUtil.GetLoginNameMqSendBody(loginName));
        }

        public void SendUserPasswordChanged(string loginName) {
            if (string.IsNullOrEmpty(loginName)) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.UserPasswordChangedRoutingKey,
                basicProperties: CreateBasicProperties(),
                body: UserMqBodyUtil.GetLoginNameMqSendBody(loginName));
        }

        public void SendUserRSAKeyUpdated(string loginName) {
            if (string.IsNullOrEmpty(loginName)) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.UserRSAKeyUpdatedRoutingKey,
                basicProperties: CreateBasicProperties(),
                body: UserMqBodyUtil.GetLoginNameMqSendBody(loginName));
        }

        private IBasicProperties CreateBasicProperties() {
            var basicProperties = _mq.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Expiration = MqKeyword.Expiration60sec;
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());

            return basicProperties;
        }
    }
}
