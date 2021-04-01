using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Core.Mq.Senders {
    public interface IMinerClientMqSender : IMqSender {
        void SendSpeeds(ClientIdIp[] clientIdIps);
        void SendMinerClientWsOpened(Guid clientId);
        void SendMinerClientWsClosed(Guid clientId);
        void SendMinerClientsWsBreathed(Guid[] clientIds);
        void SendMinerSignChanged(MinerSign minerSign);
        void SendQueryClientsForWs(string sessionId, QueryClientsForWsRequest request);
    }
}
