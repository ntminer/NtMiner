using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Core.Mq.Senders {
    public interface IMinerClientMqSender : IMqSender {
        void SendSpeeds(ClientIdIp[] clientIdIps);
        void SendMinerClientWsClosed(Guid clientId);
        void SendMinerClientsWsBreathed(Guid[] clientIds);
        void SendMinerSignsSeted(MinerSign[] minerSigns);
        void SendQueryClientsForWs(
            Guid studioId, 
            string sessionId, 
            QueryClientsForWsRequest request);
    }
}
