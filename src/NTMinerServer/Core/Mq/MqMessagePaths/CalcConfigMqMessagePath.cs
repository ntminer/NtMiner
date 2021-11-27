using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class CalcConfigMqMessagePath : AbstractMqMessagePath<CalcConfigSetInitedEvent> {
        public CalcConfigMqMessagePath(string queue) : base(queue) {
        }

        protected override Dictionary<string, Action<BasicDeliverEventArgs>> GetPaths() {
            return new Dictionary<string, Action<BasicDeliverEventArgs>> {
                [MqKeyword.CalcConfigsUpdatedRoutingKey] = ea => {
                    VirtualRoot.RaiseEvent(new CalcConfigsUpdatedMqEvent());
                }
            };
        }
    }
}
