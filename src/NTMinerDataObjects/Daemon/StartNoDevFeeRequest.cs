namespace NTMiner.Daemon {
    public class StartNoDevFeeRequest : RequestBase {
        public StartNoDevFeeRequest() { }
        public int ContextId { get; set; }
        public string MinerName { get; set; }
        public string Coin { get; set; }
        public string OurWallet { get; set; }
        public string TestWallet { get; set; }
        public string KernelName { get; set; }
    }
}
