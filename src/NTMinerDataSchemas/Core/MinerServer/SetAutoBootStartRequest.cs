namespace NTMiner.Core.MinerServer {
    public class SetAutoBootStartRequest : IRequest {
        public bool AutoBoot { get; set; }
        public bool AutoStart { get; set; }
    }
}
