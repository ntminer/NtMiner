using System;

namespace NTMiner.Core.Cpus.Impl {
    public class CpuAll : ICpuAll {
        public CpuAll() {

        }

        public int Performance { get; set; }

        public int Temperature { get; set; }

        public int HighCpuPercent { get; set; }

        public int HighCpuSeconds { get; set; }

        public DateTime LastLowCpuOn { get; set; }

        public void ResetCpu(int highCpuPercent, int highCpuSeconds) {
            this.HighCpuPercent = highCpuPercent;
            this.HighCpuSeconds = highCpuSeconds;
            this.LastLowCpuOn = DateTime.Now;
        }
    }
}
