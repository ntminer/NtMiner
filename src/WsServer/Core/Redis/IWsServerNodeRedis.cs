using NTMiner.ServerNode;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IWsServerNodeRedis : IReadOnlyWsServerNodeRedis {
        Task SetAsync(WsServerNodeState data);
        Task DeleteByAddressAsync(string address);
    }
}
