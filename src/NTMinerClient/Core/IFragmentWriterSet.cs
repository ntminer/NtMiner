using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IFragmentWriterSet {
        bool TryGetFragmentWriter(Guid writerId, out IFragmentWriter writer);
        IEnumerable<IFragmentWriter> AsEnumerable();
    }
}
