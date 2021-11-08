using System;

namespace NTMiner.Core {
    public interface ISessionSet<TSession> : ICountSet where TSession : ISession {
        void Add(TSession ntminerSession);
        TSession RemoveByWsSessionId(string wsSessionId);
        bool TryGetByClientId(Guid clientId, out TSession ntminerSession);
        bool TryGetByWsSessionId(string wsSessionId, out TSession ntminerSession);
        bool ActiveByWsSessionId(string wsSessionId, out TSession ntminerSession);
    }
}
