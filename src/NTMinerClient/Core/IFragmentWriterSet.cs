using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IFragmentWriterSet : IEnumerable<IFragmentWriter> {
        bool TryGetFragmentWriter(Guid writerId, out IFragmentWriter writer);
    }
}
