using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class MinerSignMqMessagePath : AbstractMqMessagePath<MinerSignSetInitedEvent, UserSetInitedEvent> {
        public MinerSignMqMessagePath(string queue) : base(queue) {
        }

        protected override Dictionary<string, Action<BasicDeliverEventArgs>> GetPaths() {
            return new Dictionary<string, Action<BasicDeliverEventArgs>> {
                [MqKeyword.MinerDataRemovedRoutingKey] = ea => {
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new MinerDataRemovedMqEvent(appId, clientId, ea.GetTimestamp()));
                    }
                },
                [MqKeyword.MinerDatasRemovedRoutingKey] = ea => {
                    string appId = ea.BasicProperties.AppId;
                    Guid[] clientIds = MinerClientMqBodyUtil.GetClientIdsMqReciveBody(ea.Body);
                    if (clientIds != null && clientIds.Length != 0) {
                        var timestamp = ea.GetTimestamp();
                        foreach (var clientId in clientIds) {
                            VirtualRoot.RaiseEvent(new MinerDataRemovedMqEvent(appId, clientId, timestamp));
                        }
                    }
                }
            };
        }
    }
}
