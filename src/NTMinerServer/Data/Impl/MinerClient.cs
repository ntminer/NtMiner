using System;

namespace NTMiner.Data.Impl {
    public class MinerClient {
        public Guid Id { get; set; }
        public Guid WorkId { get; set; }
        public string MinerIp { get; set; }
        public string WindowsLoginName { get; set; }

        public string WindowsPassword { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid GroupId { get; set; }
    }
}
