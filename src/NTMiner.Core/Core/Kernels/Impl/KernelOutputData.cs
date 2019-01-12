using System;

namespace NTMiner.Core.Kernels.Impl {
    public class KernelOutputData : IKernelOutput, IDbEntity<Guid> {
        public KernelOutputData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
