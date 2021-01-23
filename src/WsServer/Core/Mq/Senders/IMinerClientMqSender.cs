using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Core.Mq.Senders {
    public interface IMinerClientMqSender : IMqSender {
        void SendSpeed(string loginName, Guid clientId, string minerIp);
        void SendMinerClientWsOpened(string loginName, Guid clientId);
        void SendMinerClientWsClosed(string loginName, Guid clientId);
        void SendMinerClientWsBreathed(string loginName, Guid clientId);
        void SendChangeMinerSign(MinerSign minerSign);
        void SendQueryClientsForWs(string sessionId, QueryClientsForWsRequest request);
    }
}
