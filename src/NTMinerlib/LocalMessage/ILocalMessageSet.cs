using NTMiner.Core.MinerClient;
using System.Collections.Generic;

namespace NTMiner.LocalMessage {
    public interface ILocalMessageSet {
        IEnumerable<ILocalMessage> AsEnumerable();
    }
}
