namespace NTMiner.Service.ClientService {
    public class ClientServices {
        public readonly MinerClientServiceFace MinerClientService = MinerClientServiceFace.Instance;
        public readonly NTMinerDaemonServiceFace NTMinerDaemonService = NTMinerDaemonServiceFace.Instance;
        public readonly MinerStudioServiceFace MinerStudioService = MinerStudioServiceFace.Instance;

        internal ClientServices() { }
    }
}
