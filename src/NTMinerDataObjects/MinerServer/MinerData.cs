using System;

namespace NTMiner.MinerServer {
    public class MinerData : IMinerData {
        public MinerData() { }

        public string Id { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public string MinerName { get; set; }
        public Guid WorkId { get; set; }
        public string MinerIp { get; set; }
        public string MACAddress { get; set; }
        public string WindowsLoginName { get; set; }

        public string WindowsPassword { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid GroupId { get; set; }
    }
}
