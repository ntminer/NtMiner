using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class WsServerNodeMqMessagePath : AbstractMqMessagePath<WsServerNodeAddressSetInitedEvent> {
        public WsServerNodeMqMessagePath(string queue) : base(queue) {
        }

        protected override Dictionary<string, Action<BasicDeliverEventArgs>> GetPaths() {
            return new Dictionary<string, Action<BasicDeliverEventArgs>> {
                [MqKeyword.WsServerNodeAddedRoutingKey] = ea => {
                    string wsServerAddress = WsServerNodeMqBodyUtil.GetWsServerNodeAddressMqReceiveBody(ea.Body);
                    string appId = ea.BasicProperties.AppId;
                    VirtualRoot.RaiseEvent(new WsServerNodeAddedMqEvent(appId, wsServerAddress));
                },
                [MqKeyword.WsServerNodeRemovedRoutingKey] = ea => {
                    string wsServerAddress = WsServerNodeMqBodyUtil.GetWsServerNodeAddressMqReceiveBody(ea.Body);
                    string appId = ea.BasicProperties.AppId;
                    VirtualRoot.RaiseEvent(new WsServerNodeRemovedMqEvent(appId, wsServerAddress));
                }
            };
        }
    }
}
