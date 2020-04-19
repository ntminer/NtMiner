using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Core.Mq.Senders {
    public interface IMinerClientMqSender {
        void SendMinerClientWsOpened(string loginName, Guid clientId);
        void SendMinerClientWsClosed(string loginName, Guid clientId);
        void SendMinerClientWsBreathed(string loginName, Guid clientId);
        void SendChangeMinerSign(MinerSign minerSign);
    }
}
