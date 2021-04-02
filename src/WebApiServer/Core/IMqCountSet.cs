using NTMiner.ServerNode;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IMqCountSet {
        IEnumerable<MqCountData> AsEnumerable();
    }
}
