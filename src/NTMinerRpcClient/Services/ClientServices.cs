namespace NTMiner.Services {
    using Client;

    public class ClientServices {
        public readonly MinerClientService MinerClientService = new MinerClientService();
        public readonly NTMinerDaemonService NTMinerDaemonService = new NTMinerDaemonService();
        public readonly MinerStudioService MinerStudioService = new MinerStudioService();

        internal ClientServices() { }
    }
}
