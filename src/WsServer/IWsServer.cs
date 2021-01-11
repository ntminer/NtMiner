namespace NTMiner {
    public interface IWsServer {
        IWsSessionsAdapter MinerClientWsSessionsAdapter { get; }
        IWsSessionsAdapter MinerStudioWsSessionsAdapter { get; }

        bool Start();
        void Stop();
    }
}
