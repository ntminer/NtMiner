using NTMiner.ServerNode;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IClientTestIdDataRedis {
        Task<ClientTestIdData> GetAsync();
        Task SetAsync(ClientTestIdData clientTestId);
    }
}
