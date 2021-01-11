using System.Collections.Generic;

namespace NTMiner {
    public interface IWsSessionsAdapter {
        int Count { get; }
        IEnumerable<IWsSessionAdapter> Sessions { get; }
        bool TryGetSession(string sessionId, out IWsSessionAdapter session);
    }
}
