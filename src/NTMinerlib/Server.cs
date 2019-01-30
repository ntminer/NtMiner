namespace NTMiner {
    public static partial class Server {
        public static readonly ControlCenterServiceFace ControlCenterService = ControlCenterServiceFace.Instance;
        public static readonly AppSettingServiceFace AppSettingService = AppSettingServiceFace.Instance;
        public static readonly FileUrlServiceFace FileUrlService = FileUrlServiceFace.Instance;
        public static readonly TimeServiceFace TimeService = TimeServiceFace.Instance;
        public static readonly ProfileServiceFace ProfileService = ProfileServiceFace.Instance;
        public static readonly ReportServiceFace ReportService = ReportServiceFace.Instance;
        public static string LoginName { get; set; }

        private static string _password;
        public static string Password {
            get { return _password; }
            set {
                _password = value;
                PasswordSha1 = HashUtil.Sha1(value);
            }
        }

        public static string PasswordSha1 {
            get; private set;
        }

        public static string MinerServerHost {
            get {
                return NTMinerRegistry.GetMinerServerHost();
            }
            set {
                NTMinerRegistry.SetMinerServerHost(value);
            }
        }

        public static int MinerServerPort {
            get {
                return 3336;
            }
        }
    }
}
