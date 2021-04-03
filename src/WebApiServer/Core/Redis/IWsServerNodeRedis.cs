using NTMiner.ServerNode;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IWsServerNodeRedis : IReadOnlyWsServerNodeRedis {
        Task<List<WsServerNodeState>> GetAllAsync();
        Task ClearAsync(string[] offlines);
    }
}
