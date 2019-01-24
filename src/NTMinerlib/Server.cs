namespace NTMiner {
    public static partial class Server {
        public static readonly ControlCenterServiceFace ControlCenterService = ControlCenterServiceFace.Instance;
        public static readonly FileUrlServiceFace FileUrlService = FileUrlServiceFace.Instance;
        public static readonly TimeServiceFace TimeService = TimeServiceFace.Instance;
        public static readonly ProfileServiceFace ProfileService = ProfileServiceFace.Instance;
        public static readonly ReportServiceFace ReportService = ReportServiceFace.Instance;
        public static string LoginName { get; set; }
        public static string Password { get; set; }

        public static string MinerServerHost {
            get {
                return NTMinerRegistry.GetMinerServerHost();
            }
            set {
                NTMinerRegistry.SetMinerServerHost(value);
            }
        }

        public static string MinerServerPubKey {
            get {
                return NTMinerRegistry.GetMinerServerPubKey();
            }
            set {
                NTMinerRegistry.SetMinerServerPubKey(value);
            }
        }

        public static int MinerServerPort {
            get {
                return 3336;
            }
        }
    }
}
