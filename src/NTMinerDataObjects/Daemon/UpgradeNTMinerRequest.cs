namespace NTMiner.Daemon {
    public class UpgradeNTMinerRequest : RequestBase {
        public UpgradeNTMinerRequest() { }
        public string NTMinerFileName { get; set; }
    }
}
