using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IWsServerNodeAddressSet {
        IEnumerable<string> AsEnumerable();
    }
}
