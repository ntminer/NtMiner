using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class ReadOnlyUserMqMessagePath : AbstractMqMessagePath<UserSetInitedEvent> {
        public ReadOnlyUserMqMessagePath(string queue) : base(queue) {
        }

        protected virtual Dictionary<string, Action<BasicDeliverEventArgs>> DoGetPaths() {
            return null;
        }

        protected sealed override Dictionary<string, Action<BasicDeliverEventArgs>> GetPaths() {
            var paths = new Dictionary<string, Action<BasicDeliverEventArgs>> {
                [MqKeyword.UserAddedRoutingKey] = ea => {
                    string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                    string appId = ea.BasicProperties.AppId;
                    VirtualRoot.RaiseEvent(new UserAddedMqEvent(appId, loginName, ea.GetTimestamp()));
                },
                [MqKeyword.UserUpdatedRoutingKey] = ea => {
                    string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                    string appId = ea.BasicProperties.AppId;
                    VirtualRoot.RaiseEvent(new UserUpdatedMqEvent(appId, loginName, ea.GetTimestamp()));
                },
                [MqKeyword.UserRemovedRoutingKey] = ea => {
                    string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                    string appId = ea.BasicProperties.AppId;
                    VirtualRoot.RaiseEvent(new UserRemovedMqEvent(appId, loginName, ea.GetTimestamp()));
                },
                [MqKeyword.UserEnabledRoutingKey] = ea => {
                    string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                    string appId = ea.BasicProperties.AppId;
                    VirtualRoot.RaiseEvent(new UserEnabledMqEvent(appId, loginName, ea.GetTimestamp()));
                },
                [MqKeyword.UserDisabledRoutingKey] = ea => {
                    string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                    string appId = ea.BasicProperties.AppId;
                    VirtualRoot.RaiseEvent(new UserDisabledMqEvent(appId, loginName, ea.GetTimestamp()));
                },
                [MqKeyword.UserPasswordChangedRoutingKey] = ea => {
                    string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                    string appId = ea.BasicProperties.AppId;
                    VirtualRoot.RaiseEvent(new UserPasswordChangedMqEvent(appId, loginName, ea.GetTimestamp()));
                },
                [MqKeyword.UserRSAKeyUpdatedRoutingKey] = ea => {
                    string loginName = UserMqBodyUtil.GetLoginNameMqReceiveBody(ea.Body);
                    string appId = ea.BasicProperties.AppId;
                    VirtualRoot.RaiseEvent(new UserRSAKeyUpdatedMqEvent(appId, loginName, ea.GetTimestamp()));
                }
            };
            var dic = DoGetPaths();
            if (dic != null) {
                foreach (var kv in dic) {
                    paths[kv.Key] = kv.Value;
                }
            }
            return paths;
        }
    }
}
