namespace NTMiner.MinerServer {
    public class ClientCount {
        public int OnlineCount { get; set; }
        public int MiningCount { get; set; }
    }

    public class ClientCoinCount {
        public int MainCoinOnlineCount { get; set; }
        public int MainCoinMiningCount { get; set; }
        public int DualCoinOnlineCount { get; set; }
        public int DualCoinMiningCount { get; set; }
    }
}
