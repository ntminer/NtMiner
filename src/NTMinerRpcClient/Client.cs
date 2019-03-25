namespace NTMiner {
    public static partial class Client {
        public static readonly MinerClientServiceFace MinerClientService = MinerClientServiceFace.Instance;
        public static readonly NTMinerDaemonServiceFace NTMinerDaemonService = NTMinerDaemonServiceFace.Instance;
        public static readonly MinerStudioServiceFace MinerStudioService = MinerStudioServiceFace.Instance;
    }
}
