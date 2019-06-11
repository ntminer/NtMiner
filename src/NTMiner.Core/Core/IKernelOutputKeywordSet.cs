using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IKernelOutputKeywordSet : IEnumerable<IKernelOutputKeyword> {
        bool Contains(Guid id);
        bool TryGetKernelOutputPicker(Guid id, out IKernelOutputKeyword eventType);
    }
}
