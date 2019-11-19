using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.ServerMessage {
    public interface IServerMessageSet {
        List<ServerMessageData> GetServerMessages(DateTime timeStamp);
        IEnumerable<IServerMessage> AsEnumerable();
    }
}
