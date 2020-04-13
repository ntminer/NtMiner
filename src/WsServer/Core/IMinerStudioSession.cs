using NTMiner.Ws;

namespace NTMiner.Core {
    public interface IMinerStudioSession : ISession {
        bool IsValid(WsMessage message);
    }
}
