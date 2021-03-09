using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IServerMessageSet {
        List<ServerMessageData> GetServerMessages(DateTime timeStamp);
        IEnumerable<IServerMessage> AsEnumerable();
    }
}
