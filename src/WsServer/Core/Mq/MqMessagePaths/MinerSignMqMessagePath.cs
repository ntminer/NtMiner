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
                    string minerId = MinerClientMqBodyUtil.GetMinerIdMqReciveBody(ea.Body);
                    if (!string.IsNullOrEmpty(minerId) && ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new MinerDataRemovedMqEvent(appId, minerId, clientId, ea.GetTimestamp()));
                    }
                }
            };
        }
    }
}
