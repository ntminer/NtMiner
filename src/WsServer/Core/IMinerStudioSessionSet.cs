using NTMiner.Ws;

namespace NTMiner.Core {
    public interface IMinerStudioSessionSet : ISessionSet<IMinerStudioSession> {
        void SendToMinerStudioAsync(string loginName, WsMessage message);
    }
}
