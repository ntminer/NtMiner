using System;

namespace NTMiner.Core {
    public interface ICoinShareSet {
        ICoinShare GetOrCreate(Guid coinId);
    }
}
