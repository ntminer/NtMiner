using System;

namespace NTMiner.ServerNode {
    public class RemoteIpEntryDto : IRemoteIpEntry {
        public RemoteIpEntryDto() { }

        public string RemoteIp { get; set; }

        public int ActionTimes { get; set; }

        public DateTime LastActionOn { get; set; }

        public long ActionSpeed { get; set; }
    }
}
