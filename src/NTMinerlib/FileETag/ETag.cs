using System;

namespace NTMiner.FileETag {
    public class ETag : IETag {
        public ETag() {
        }

        /// <summary>
        /// Key往往是文件名，是相对于GlobalDir的相对文件名
        /// </summary>
        [LiteDB.BsonId]
        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
