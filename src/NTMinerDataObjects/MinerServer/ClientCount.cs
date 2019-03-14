namespace NTMiner.MinerServer {
    public class ClientCount {
        public void Update(int onlineCount, int miningCount) {
            this.OnlineCount = onlineCount;
            this.MiningCount = miningCount;
        }
        public int OnlineCount { get; private set; }
        public int MiningCount { get; private set; }
    }

    public class ClientCoinCount {
        public int MainCoinOnlineCount { get; set; }
        public int MainCoinMiningCount { get; set; }
        public int DualCoinOnlineCount { get; set; }
        public int DualCoinMiningCount { get; set; }
    }
}
