using System;

namespace NTMiner.MinerServer {
    public class LoginData : RequestBase {
        public LoginData() { }
        public Guid WorkId { get; set; }

        public Guid ClientId { get; set; }

        public string MinerName { get; set; }

        public string Version { get; set; }

        public string GpuInfo { get; set; }
    }
}
