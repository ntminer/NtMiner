using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserAddedMqEvent(appId, loginName, ea.GetTimestamp()));
                    }
                    break;
                case MqKeyword.UserUpdatedRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserUpdatedMqEvent(appId, loginName, ea.GetTimestamp()));
                    }
                    break;
                case MqKeyword.UserRemovedRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserRemovedMqEvent(appId, loginName, ea.GetTimestamp()));
                    }
                    break;
                case MqKeyword.UserEnabledRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserEnabledMqEvent(appId, loginName, ea.GetTimestamp()));
                    }
                    break;
                case MqKeyword.UserDisabledRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserDisabledMqEvent(appId, loginName, ea.GetTimestamp()));
                    }
                    break;
                case MqKeyword.UserPasswordChangedRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserPasswordChangedMqEvent(appId, loginName, ea.GetTimestamp()));
                    }
                    break;
                case MqKeyword.UserRSAKeyUpdatedRoutingKey: {
                        string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                        string appId = ea.BasicProperties.AppId;
                        VirtualRoot.RaiseEvent(new UserRSAKeyUpdatedMqEvent(appId, loginName, ea.GetTimestamp()));
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
