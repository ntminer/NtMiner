using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Core.Mq.Senders {
    public interface IMinerClientMqSender : IMqSender {
        void SendSpeed(string loginName, Guid clientId, string minerIp);
        void SendMinerClientWsOpened(Guid clientId);
        void SendMinerClientWsClosed(Guid clientId);
        void SendMinerClientWsBreathed(Guid clientId);
        void SendMinerClientsWsBreathed(Guid[] clientIds);
        void SendChangeMinerSign(MinerSign minerSign);
        void SendQueryClientsForWs(string sessionId, QueryClientsForWsRequest request);
    }
}
