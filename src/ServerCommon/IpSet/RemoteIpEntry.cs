using NTMiner.ServerNode;
using System;
using System.Collections.Generic;
using System.Net;

namespace NTMiner.IpSet {
    public class RemoteIpEntry : IRemoteIpEntry {
        public RemoteIpEntry(IPAddress remoteIp) {
            this.RemoteIp = remoteIp;
            this.DateTimes = new Queue<DateTime>();
        }

        public IPAddress RemoteIp { get; private set; }
        string IRemoteIpEntry.RemoteIp {
            get { return this.RemoteIp.ToString(); }
        }
        public int ActionTimes { get; private set; }
        public DateTime LastActionOn { get; private set; }
        public Queue<DateTime> DateTimes { get; private set; }

        public long ActionSpeed {
            get {
                var data = DateTimes.ToArray();
                if (data.Length < 2) {
                    return 0;
                }
                double seconds = (LastActionOn - data[0]).TotalSeconds;
                if (seconds == 0) {
                    return long.MaxValue;
                }
                var speed = (long)(data.Length / seconds);
                if (speed < 0) {
                    return long.MaxValue;
                }
                return speed;
            }
        }

        public void IncActionTimes() {
            this.LastActionOn = DateTime.Now;
            this.ActionTimes++;
            this.DateTimes.Enqueue(this.LastActionOn);
            while (this.DateTimes.Count > 10) {
                this.DateTimes.Dequeue();
            }
        }

        public RemoteIpEntryDto ToDto() {
            return new RemoteIpEntryDto {
                ActionSpeed = this.ActionSpeed,
                ActionTimes = this.ActionTimes,
                LastActionOn = this.LastActionOn,
                RemoteIp = ((IRemoteIpEntry)this).RemoteIp
            };
        }
    }
}
