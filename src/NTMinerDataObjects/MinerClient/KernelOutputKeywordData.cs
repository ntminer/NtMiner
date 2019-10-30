using System;

namespace NTMiner.MinerClient {
    public class KernelOutputKeywordData : IKernelOutputKeyword, IDbEntity<Guid> {
        public KernelOutputKeywordData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }
        public Guid KernelOutputId { get; set; }

        public string MessageType { get; set; }

        public string Keyword { get; set; }
    }
}
