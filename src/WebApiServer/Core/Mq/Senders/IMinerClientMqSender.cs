using NTMiner.Core.MinerServer;

namespace NTMiner.Core.Mq.Senders {
    public interface IMinerClientMqSender : IMqSender {
        void SendMinerDataAdded(string minerId);
        void SendMinerDataRemoved(string minerId);
        void SendMinerSignChanged(string minerId);
        void SendResponseClientsForWs(string wsServerIp, string loginName, string sessionId, QueryClientsResponse response);
    }
}
