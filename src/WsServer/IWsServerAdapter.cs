namespace NTMiner {
    public interface IWsServerAdapter {
        IWsSessionsAdapter MinerClientWsSessions { get; }
        IWsSessionsAdapter MinerStudioWsSessions { get; }

        bool Start();
        void Stop();
    }
}
