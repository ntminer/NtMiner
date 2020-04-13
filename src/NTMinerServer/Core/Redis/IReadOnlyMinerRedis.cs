using NTMiner.Core.MinerServer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IReadOnlyMinerRedis {
        Task<List<MinerData>> GetAllAsync();
        Task<MinerData> GetByIdAsync(string minerId);
    }
}
