using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class UserMqMessagePath : ReadOnlyUserMqMessagePath {
        public UserMqMessagePath(string queue) : base(queue) {
        }

        protected override void DoBuild(IModel channel) {
            base.DoBuild(channel);
            channel.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.UpdateUserRSAKeyRoutingKey, arguments: null);
        }

        protected override void DoGo(BasicDeliverEventArgs ea) {
            base.DoGo(ea);
            switch (ea.RoutingKey) {
                case MqKeyword.UpdateUserRSAKeyRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        var key = UserMqBodyUtil.GetUpdateUserRSAKeyMqReceiveBody(ea.Body);
                        VirtualRoot.Execute(new UpdateUserRSAKeyMqMessage(appId, loginName, timestamp, key));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
