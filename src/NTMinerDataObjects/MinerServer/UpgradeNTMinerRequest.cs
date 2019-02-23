namespace NTMiner.MinerServer {
    public class UpgradeNTMinerRequest : RequestBase {
        public string ClientIp { get; set; }
        public string NTMinerFileName { get; set; }
    }
}
