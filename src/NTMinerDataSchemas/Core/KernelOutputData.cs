using System;

namespace NTMiner.Core {
    public class KernelOutputData : IKernelOutput, IDbEntity<Guid> {
        public KernelOutputData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string KernelRestartKeyword { get; set; }

        public bool IsDualInSameLine { get; set; }

        public string TotalSpeedPattern { get; set; }

        public string TotalSharePattern { get; set; }

        public string AcceptSharePattern { get; set; }

        public string FoundOneShare { get; set; }

        public string AcceptOneShare { get; set; }

        public string RejectSharePattern { get; set; }

        public string RejectOneShare { get; set; }

        public string RejectPercentPattern { get; set; }

        public string GpuSpeedPattern { get; set; }

        public string GpuGotOneIncorrectShare { get; set; }


        public string DualTotalSpeedPattern { get; set; }

        public string DualTotalSharePattern { get; set; }

        public string DualAcceptSharePattern { get; set; }

        public string DualAcceptOneShare { get; set; }

        public string DualRejectSharePattern { get; set; }

        public string DualRejectOneShare { get; set; }

        public string DualRejectPercentPattern { get; set; }

        public string DualGpuSpeedPattern { get; set; }

        public string PoolDelayPattern { get; set; }

        public string DualPoolDelayPattern { get; set; }

        public string SpeedUnit { get; set; }

        public string DualSpeedUnit { get; set; }

        public int GpuBaseIndex { get; set; }

        public bool IsMapGpuIndex { get; set; }
    }
}
