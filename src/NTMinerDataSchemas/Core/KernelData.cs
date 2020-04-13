using System;

namespace NTMiner.Core {
    public class KernelData : IKernel, IDbEntity<Guid> {
        public KernelData() {
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Code { get; set; }

        public Guid BrandId { get; set; }

        public string Version { get; set; }

        public long PublishOn { get; set; }

        public string Package { get; set; }

        public long Size { get; set; }

        public PublishStatus PublishState { get; set; }

        public string Notice { get; set; }

        public Guid KernelInputId { get; set; }
        public Guid KernelOutputId { get; set; }
    }
}
