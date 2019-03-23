using System;

namespace NTMiner.Core {
    public class PoolKernelData : IPoolKernel, IDbEntity<Guid> {
        public PoolKernelData() {
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid PoolId { get; set; }

        public Guid KernelId { get; set; }

        public string Args { get; set; }

        public string Description { get; set; }
    }
}
