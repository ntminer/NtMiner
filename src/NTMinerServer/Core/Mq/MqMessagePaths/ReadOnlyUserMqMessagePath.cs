using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class ReadOnlyUserMqMessagePath : AbstractMqMessagePath<UserSetInitedEvent> {
        public ReadOnlyUserMqMessagePath(string queue) : base(queue) {
        }

        protected virtual void DoBuild(IModel channel) {
        }

        protected internal override void Build(IModel channel) {
            DoBuild(channel);
            channel.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.UserAddedRoutingKey, arguments: null);
            channel.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.UserUpdatedRoutingKey, arguments: null);
            channel.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.UserRemovedRoutingKey, arguments: null);
            channel.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.UserEnabledRoutingKey, arguments: null);
            channel.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.UserDisabledRoutingKey, arguments: null);
            channel.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.UserPasswordChangedRoutingKey, arguments: null);
            channel.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.UserRSAKeyUpdatedRoutingKey, arguments: null);

            NTMinerConsole.UserOk("UserMq QueueBind成功");
        }

        protected virtual bool DoGo(BasicDeliverEventArgs ea) {
            return false;
        }

        public override bool Go(BasicDeliverEventArgs ea) {
            bool baseR = DoGo(ea);
            bool r = true;
            switch (ea.RoutingKey) {
                case MqKeyword.UserAddedRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserAddedMqMessage(appId, loginName, timestamp));
                    }
                    break;
                case MqKeyword.UserUpdatedRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserUpdatedMqMessage(appId, loginName, timestamp));
                    }
                    break;
                case MqKeyword.UserRemovedRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserRemovedMqMessage(appId, loginName, timestamp));
                    }
                    break;
                case MqKeyword.UserEnabledRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserEnabledMqMessage(appId, loginName, timestamp));
                    }
                    break;
                case MqKeyword.UserDisabledRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserDisabledMqMessage(appId, loginName, timestamp));
                    }
                    break;
                case MqKeyword.UserPasswordChangedRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserPasswordChangedMqMessage(appId, loginName, timestamp));
                    }
                    break;
                case MqKeyword.UserRSAKeyUpdatedRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserRSAKeyUpdatedMqMessage(appId, loginName, timestamp));
                    }
                    break;
                default:
                    r = false;
                    break;
            }
            return baseR || r;
        }
    }
}
