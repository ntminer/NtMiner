using System;

namespace NTMiner.FileETag.Impl {
    public class ETag : IETag, IDbEntity<Guid> {
        public ETag() {
            this.Id = Guid.NewGuid();
        }

        public ETag(IETag data) {
            this.Id = data.GetId();
            this.Key = data.Key;
            this.Value = data.Value;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
