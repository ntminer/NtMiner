using System;

namespace NTMiner.Core {
    public interface IKey {
        string PublicKey { get; }
        string Name { get; }
        string Description { get; }
        DateTime CreatedOn { get; }
    }
}
