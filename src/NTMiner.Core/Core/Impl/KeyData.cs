using System;

namespace NTMiner.Core.Impl {
    public class KeyData : IKey {
        [LiteDB.BsonId]
        public string PublicKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
