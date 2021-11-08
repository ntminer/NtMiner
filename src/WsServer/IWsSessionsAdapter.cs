namespace NTMiner {
    public interface IWsSessionsAdapter : ICountSet {
        bool TryGetSession(string sessionId, out IWsSessionAdapter session);
    }
}
