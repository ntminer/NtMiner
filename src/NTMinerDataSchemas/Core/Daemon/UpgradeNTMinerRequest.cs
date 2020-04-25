namespace NTMiner.Core.Daemon {
    public class UpgradeNTMinerRequest : IRequest {
        public string NTMinerFileName { get; set; }

        public UpgradeNTMinerRequest() { }
    }
}
