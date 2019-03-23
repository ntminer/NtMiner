using System;

namespace NTMiner.Core {
    public class KernelOutputTranslaterData : IKernelOutputTranslater, IDbEntity<Guid>, ISortable {
        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid KernelOutputId { get; set; }

        public string RegexPattern { get; set; }

        public string Replacement { get; set; }

        public string Color { get; set; }

        public int SortNumber { get; set; }

        public bool IsPre { get; set; }
    }
}
