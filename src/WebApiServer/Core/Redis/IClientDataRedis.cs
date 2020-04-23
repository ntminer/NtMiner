using NTMiner.Core.MinerServer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IClientDataRedis {
        Task<List<ClientData>> GetAllAsync();
        Task<ClientData> GetByIdAsync(string minerId);
        Task SetAsync(ClientData data);
        Task DeleteAsync(ClientData data);
    }
}
