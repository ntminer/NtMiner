using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IFileWriterSet : IEnumerable<IFileWriter> {
        bool TryGetFileWriter(Guid fileWriterId, out IFileWriter fileWriter);
    }
}
