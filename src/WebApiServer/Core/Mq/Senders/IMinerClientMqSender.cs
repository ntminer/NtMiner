using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Core.Mq.Senders {
    public interface IMinerClientMqSender : IMqSender {
        void SendMinerDataRemoved(string minerId, Guid clientId);
        void SendResponseClientsForWs(
            string wsServerIp, 
            string loginName, 
            string sessionId, 
            string mqCorrelationId, 
            QueryClientsResponse response);
    }
}
