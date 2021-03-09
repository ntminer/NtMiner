using NTMiner.Core.Daemon;
using System;

namespace NTMiner.Core.Mq.Senders {
    public interface IOperationMqSender : IMqSender {
        void SendStartWorkMine(string loginName, Guid clientId, WorkRequest request);
    }
}
