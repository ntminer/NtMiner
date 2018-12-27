using Microsoft.Win32;

namespace NTMiner {
    public static partial class Server {
        public static readonly ControlCenterServiceFace ControlCenterService = ControlCenterServiceFace.Instance;
        public static readonly FileUrlServiceFace FileUrlService = FileUrlServiceFace.Instance;
        public static readonly TimeServiceFace TimeService = TimeServiceFace.Instance;
        public static readonly ProfileServiceFace ProfileService = ProfileServiceFace.Instance;
        public static readonly ReportServiceFace ReportService = ReportServiceFace.Instance;
        public static string LoginName { get; set; }
        public static string Password { get; set; }

        public const string MINER_SERVER_HOST = "server.ntminer.com";
        public const string MINER_SERVER_PUBKEY = "AwEAAdRA5vw4CvIXKynkRa4HLJaFTwUzIcOWybnht5ZV/4BOu6g4cmrDwmRLrMM0rTjUJQ==";
        public static string MinerServerHost {
            get {
                object minerServerHostValue = Windows.Registry.GetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "MinerServerHost");
                if (minerServerHostValue == null) {
                    return MINER_SERVER_HOST;
                }
                return (string)minerServerHostValue;
            }
            set {
                if (string.IsNullOrEmpty(value)) {
                    value = MINER_SERVER_HOST;
                }
                Windows.Registry.SetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "MinerServerHost", value);
            }
        }

        public static string MinerServerPubKey {
            get {
                object minerServerPubKeyValue = Windows.Registry.GetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "MinerServerPubKey");
                if (minerServerPubKeyValue == null) {
                    return MINER_SERVER_PUBKEY;
                }
                return (string)minerServerPubKeyValue;
            }
            set {
                if (string.IsNullOrEmpty(value)) {
                    value = MINER_SERVER_PUBKEY;
                }
                Windows.Registry.SetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "MinerServerPubKey", value);
            }
        }

        public static int MinerServerPort {
            get {
                return 3336;
            }
        }
    }
}
