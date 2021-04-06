using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class ClientTestIdMqMessagePath : AbstractMqMessagePath {
        public ClientTestIdMqMessagePath(string queue) : base(queue) {
        }

        public override bool IsReadyToBuild {
            get {
                return true;
            }
        }

        protected override Dictionary<string, Action<BasicDeliverEventArgs>> GetPaths() {
            return new Dictionary<string, Action<BasicDeliverEventArgs>> {
                [MqKeyword.RefreshMinerTestIdRoutingKey] = ea => {
                    VirtualRoot.Execute(new RefreshMinerTestIdMqCommand(ea.GetTimestamp()));
                }
            };
        }
    }
}
