using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IFileWriterSet {
        bool TryGetFileWriter(Guid writerId, out IFileWriter writer);
        IEnumerable<IFileWriter> AsEnumerable();
    }
}
