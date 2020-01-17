namespace NTMiner.Service.ClientService {
    public class ClientServices {
        public readonly MinerClientService MinerClientService = MinerClientService.Instance;
        public readonly NTMinerDaemonService NTMinerDaemonService = NTMinerDaemonService.Instance;
        public readonly MinerStudioService MinerStudioService = MinerStudioService.Instance;

        internal ClientServices() { }
    }
}
