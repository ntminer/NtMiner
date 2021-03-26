using System;

namespace NTMiner.IdGenerator {
    public class DefaultIdGenerator : IIdGenerator {
        public string Generate() {
            return Guid.NewGuid().ToString("N");
        }
    }
}
