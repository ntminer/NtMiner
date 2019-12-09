namespace NTMiner {
    public partial class Client {
        public readonly MinerClientServiceFace MinerClientService = MinerClientServiceFace.Instance;
        public readonly NTMinerDaemonServiceFace NTMinerDaemonService = NTMinerDaemonServiceFace.Instance;
        public readonly MinerStudioServiceFace MinerStudioService = MinerStudioServiceFace.Instance;

        internal Client() { }
    }
}
