using NTMiner.Core.MinerServer;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class MinerClientMqMessagePath : AbstractMqMessagePath<ClientSetInitedEvent> {
        public MinerClientMqMessagePath(string queue) : base(queue) {
        }

        protected override Dictionary<string, Action<BasicDeliverEventArgs>> GetPaths() {
            return new Dictionary<string, Action<BasicDeliverEventArgs>> {
                [MqKeyword.SpeedsRoutingKey] = ea => {
                    ClientIdIp[] clientIdIps = MinerClientMqBodyUtil.GetClientIdIpsMqReciveBody(ea.Body);
                    string appId = ea.BasicProperties.AppId;
                    VirtualRoot.RaiseEvent(new SpeedDatasMqEvent(appId, clientIdIps, ea.GetTimestamp()));
                },
                [MqKeyword.MinerSignsSetedRoutingKey] = ea => {
                    string appId = ea.BasicProperties.AppId;
                    MinerSign[] minerSigns = MinerClientMqBodyUtil.GetMinerSignsMqReceiveBody(ea.Body);
                    if (minerSigns != null && minerSigns.Length != 0) {
                        VirtualRoot.RaiseEvent(new MinerSignsSetedMqEvent(appId, minerSigns, ea.GetTimestamp()));
                    }
                },
                [MqKeyword.QueryClientsForWsRoutingKey] = ea => {
                    string appId = ea.BasicProperties.AppId;
                    string mqMessageId = ea.BasicProperties.MessageId;
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string sessionId = ea.BasicProperties.ReadHeaderString(MqKeyword.SessionIdHeaderName);
                    QueryClientsForWsRequest query = MinerClientMqBodyUtil.GetQueryClientsForWsMqReceiveBody(ea.Body);
                    if (query != null) {
                        VirtualRoot.Execute(new QueryClientsForWsMqCommand(appId, mqMessageId, ea.GetTimestamp(), loginName, sessionId, query));
                    }
                }
            };
        }
    }
}
