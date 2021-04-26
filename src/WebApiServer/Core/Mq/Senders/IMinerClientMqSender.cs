using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Core.Mq.Senders {
    public interface IMinerClientMqSender : IMqSender {
        void SendMinerDataRemoved(string minerId, Guid clientId);
        void SendMinerDatasRemoved(Guid[] clientIds);
        void SendResponseClientsForWs(
            string wsServerIp, 
            string loginName, 
            Guid studioId,
            string sessionId, 
            string mqCorrelationId, 
            QueryClientsResponse response);
        void SendResponseClientsForWs(string wsServerIp, string mqCorrelationId, QueryClientsResponseEx[] responses);
    }
}
