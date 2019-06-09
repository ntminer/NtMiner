using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IMinerEventSet {
        IEnumerable<IMinerEvent> Query(Guid? typeId, string keyword, DateTime? leftTime, DateTime? rightTime);
    }
}
