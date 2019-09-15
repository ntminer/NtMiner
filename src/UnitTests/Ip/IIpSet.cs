using System.Collections.Generic;
using System.Net;

namespace NTMiner.Ip {
    public interface IIpSet : IEnumerable<IPAddress> {
        bool Contains(IPAddress ip);
    }
}
