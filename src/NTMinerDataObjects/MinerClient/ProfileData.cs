namespace NTMiner.MinerClient {
    public class ProfileData {
        public ProfileData() { }
        public string MinerName { get; set; }
        public bool IsAutoBoot { get; set; }
        public bool IsAutoStart { get; set; }
        public bool IsAutoRestartKernel { get; set; }
        public string CoinCode { get; set; }
        public bool IsNoShareRestartKernel { get; set; }
        public int NoShareRestartKernelMinutes { get; set; }
        public bool IsPeriodicRestartKernel { get; set; }
        public int PeriodicRestartKernelHours { get; set; }
        public bool IsPeriodicRestartComputer { get; set; }
        public int PeriodicRestartComputerHours { get; set; }

        public string MainCoinPool { get; set; }
        public string MainCoinWallet { get; set; }
        public bool IsMainCoinPoolIsUserMode { get; set; }
        public string MainCoinPoolUserName { get; set; }

        public bool IsDualCoinEnabled { get; set; }

        public string DualCoinCode { get; set; }

        public double DualCoinWeight { get; set; }

        public bool IsAutoDualWeight { get; set; }

        public string DualCoinPool { get; set; }
        public string DualCoinWallet { get; set; }
        public bool IsDualCoinPoolIsUserMode { get; set; }
        public string DualCoinPoolUserName { get; set; }
    }
}
