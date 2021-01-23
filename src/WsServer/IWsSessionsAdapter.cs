namespace NTMiner {
    public interface IWsSessionsAdapter {
        int Count { get; }
        bool TryGetSession(string sessionId, out IWsSessionAdapter session);
    }
}
