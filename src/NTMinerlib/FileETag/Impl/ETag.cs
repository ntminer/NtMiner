using System;

namespace NTMiner.FileETag.Impl {
    public class ETag : IETag, IDbEntity<Guid> {
        public ETag() {
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
