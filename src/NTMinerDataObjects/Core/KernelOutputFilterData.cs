using System;

namespace NTMiner.Core {
    public class KernelOutputFilterData : IKernelOutputFilter, IDbEntity<Guid> {
        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid KernelOutputId { get; set; }

        public string RegexPattern { get; set; }
    }
}
