using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.LocalMessage {
    public interface ILocalMessageSet : IEnumerable<ILocalMessage> {
        int Count { get; }
        void Add(string channel, string provider, string messageType, string content);
        void Clear();
    }
}
