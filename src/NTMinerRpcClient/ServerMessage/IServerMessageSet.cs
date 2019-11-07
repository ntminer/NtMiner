using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.ServerMessage {
    public interface IServerMessageSet : IEnumerable<IServerMessage> {
        List<IServerMessage> GetServerMessages(DateTime timeStamp);
    }
}
