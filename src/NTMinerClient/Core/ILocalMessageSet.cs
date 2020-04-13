using NTMiner.Core.MinerClient;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ILocalMessageSet {
        ILocalMessageDtoSet LocalMessageDtoSet { get; }
        IEnumerable<ILocalMessage> AsEnumerable();
    }
}
