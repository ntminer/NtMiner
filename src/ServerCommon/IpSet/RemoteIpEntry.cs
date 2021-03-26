using System;
using System.Collections.Generic;
using System.Net;

namespace NTMiner.IpSet {
    public class RemoteIpEntry {
        public RemoteIpEntry(IPAddress remoteIp) {
            this.RemoteIp = remoteIp;
            this.DateTimes = new Queue<DateTime>();
        }

        public IPAddress RemoteIp { get; private set; }
        public int ActionTimes { get; private set; }
        public DateTime LastActionOn { get; private set; }
        public Queue<DateTime> DateTimes { get; private set; }

        public void IncActionTimes() {
            this.LastActionOn = DateTime.Now;
            this.ActionTimes++;
            this.DateTimes.Enqueue(this.LastActionOn);
            while (this.DateTimes.Count > 10) {
                this.DateTimes.Dequeue();
            }
        }
    }
}
