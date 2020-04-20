using NTMiner.Ws;
using System;

namespace NTMiner.Core {
    public interface IMinerClientSessionSet : ISessionSet<IMinerClientSession> {
        void SendToMinerClientAsync(Guid clientId, WsMessage message);
        void SendToWsClientAsync(string wsSessionId, WsMessage message);
    }
}
