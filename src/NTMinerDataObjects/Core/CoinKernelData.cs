using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public class CoinKernelData : ICoinKernel, IDbEntity<Guid> {
        public CoinKernelData() {
            this.EnvironmentVariables = new List<EnvironmentVariable>();
            this.InputSegments = new List<InputSegment>();
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid CoinId { get; set; }

        public Guid KernelId { get; set; }

        public SupportedGpu SupportedGpu { get; set; }

        public int SortNumber { get; set; }

        public Guid DualCoinGroupId { get; set; }

        public string Args { get; set; }

        public string Notice { get; set; }

        public List<EnvironmentVariable> EnvironmentVariables { get; set; }

        public List<InputSegment> InputSegments { get; set; }
    }
}
