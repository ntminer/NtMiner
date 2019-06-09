using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IMinerEventSet {
        IEnumerable<IMinerEvent> GetEvents(Guid? typeId, string keyword);
    }
}
