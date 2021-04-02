using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class UserMqMessagePath : ReadOnlyUserMqMessagePath {
        public UserMqMessagePath(string queue) : base(queue) {
        }

        protected override Dictionary<string, Action<BasicDeliverEventArgs>> DoGetPaths() {
            return new Dictionary<string, Action<BasicDeliverEventArgs>> {
                [MqKeyword.UpdateUserRSAKeyRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    var key = UserMqBodyUtil.GetUpdateUserRSAKeyMqReceiveBody(ea.Body);
                    VirtualRoot.Execute(new UpdateUserRSAKeyMqCommand(appId, loginName, ea.GetTimestamp(), key));
                }
            };
        }
    }
}
