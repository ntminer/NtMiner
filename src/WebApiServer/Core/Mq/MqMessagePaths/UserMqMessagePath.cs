using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class UserMqMessagePath : ReadOnlyUserMqMessagePath {
        public UserMqMessagePath(string queue) : base(queue) {
        }

        protected override void DoBuild(IModel channel) {
            base.DoBuild(channel);
            channel.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.UpdateUserRSAKeyRoutingKey, arguments: null);
        }

        protected override bool DoGo(BasicDeliverEventArgs ea) {
            bool baseR = base.DoGo(ea);
            bool r = true;
            switch (ea.RoutingKey) {
                case MqKeyword.UpdateUserRSAKeyRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        string appId = ea.BasicProperties.AppId;
                        var key = UserMqBodyUtil.GetUpdateUserRSAKeyMqReceiveBody(ea.Body);
                        VirtualRoot.Execute(new UpdateUserRSAKeyMqCommand(appId, loginName, ea.GetTimestamp(), key));
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
