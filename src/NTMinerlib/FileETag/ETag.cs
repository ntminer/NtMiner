using System;

namespace NTMiner.FileETag {
    public class ETag : IETag, IDbEntity<Guid> {
        public ETag() {
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        /// <summary>
        /// Key往往是文件名，是相对于GlobalDir的相对文件名
        /// </summary>
        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
