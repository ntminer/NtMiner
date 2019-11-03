using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.ServerMessage {
    public interface IServerMessageSet : IEnumerable<IServerMessage> {
        int Count { get; }
        void Add(string provider, string messageType, string content);
        List<IServerMessage> GetServerMessages(DateTime timeStamp);
        void Clear();
    }
}
