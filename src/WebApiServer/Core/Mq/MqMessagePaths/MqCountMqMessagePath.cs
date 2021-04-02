using NTMiner.ServerNode;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class MqCountMqMessagePath : AbstractMqMessagePath {
        public MqCountMqMessagePath(string queue) : base(queue) {
        }

        public override bool IsReadyToBuild {
            get {
                return true;
            }
        }

        protected override Dictionary<string, Action<BasicDeliverEventArgs>> GetPaths() {
            return new Dictionary<string, Action<BasicDeliverEventArgs>> {
                [MqKeyword.MqCountRoutingKey] = ea => {
                    string appId = ea.BasicProperties.AppId;
                    MqCountData data = MqCountMqBodyUtil.GetMqCountMqReceiveBody(ea.Body);
                    if (data != null) {
                        VirtualRoot.RaiseEvent(new MqCountReceivedMqEvent(appId, data, ea.GetTimestamp()));
                    }
                }
            };
        }
    }
}
