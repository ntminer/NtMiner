using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.ServerMessage {
    public interface IServerMessageSet : IEnumerable<IServerMessage> {
        void Add(string provider, string messageType, string content);
        void AddOrUpdate(IServerMessage entity);
        void Remove(Guid id);
        List<IServerMessage> GetServerMessages(DateTime timeStamp);
        void Clear();
    }
}
