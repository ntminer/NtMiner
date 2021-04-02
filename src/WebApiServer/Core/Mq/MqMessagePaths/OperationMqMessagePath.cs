using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class OperationMqMessagePath : AbstractMqMessagePath {
        public OperationMqMessagePath(string queue) : base(queue) {
        }

        public override bool IsReadyToBuild {
            get {
                return true;
            }
        }

        protected override Dictionary<string, Action<BasicDeliverEventArgs>> GetPaths() {
            return new Dictionary<string, Action<BasicDeliverEventArgs>> {
                [MqKeyword.StartMineRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        Guid workId = OperationMqBodyUtil.GetStartMineMqReceiveBody(ea.Body);
                        VirtualRoot.RaiseEvent(new StartMineMqEvent(appId, loginName, ea.GetTimestamp(), clientId, workId));
                    }
                }
            };
        }
    }
}
