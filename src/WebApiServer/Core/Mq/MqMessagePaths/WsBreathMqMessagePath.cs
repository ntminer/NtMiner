using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class WsBreathMqMessagePath : AbstractMqMessagePath<ClientSetInitedEvent> {
        public WsBreathMqMessagePath(string queue) : base(queue) {
        }

        protected override Dictionary<string, Action<BasicDeliverEventArgs>> GetPaths() {
            return new Dictionary<string, Action<BasicDeliverEventArgs>> {
                [MqKeyword.MinerClientWsClosedRoutingKey] = ea => {
                    string appId = ea.BasicProperties.AppId;
                    Guid clientId = MinerClientMqBodyUtil.GetClientIdMqReciveBody(ea.Body);
                    if (clientId != Guid.Empty) {
                        VirtualRoot.RaiseEvent(new MinerClientWsClosedMqEvent(appId, clientId, ea.GetTimestamp()));
                    }
                },
                [MqKeyword.MinerClientsWsBreathedRoutingKey] = ea => {
                    string appId = ea.BasicProperties.AppId;
                    Guid[] clientIds = MinerClientMqBodyUtil.GetClientIdsMqReciveBody(ea.Body);
                    if (clientIds != null && clientIds.Length != 0) {
                        VirtualRoot.RaiseEvent(new MinerClientsWsBreathedMqEvent(appId, clientIds, ea.GetTimestamp()));
                    }
                }
            };
        }
    }
}
