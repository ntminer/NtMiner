using NTMiner.Core.MinerServer;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IMinerDataRedis : IReadOnlyMinerDataRedis {
        Task SetAsync(MinerData data);
        Task UpdateAsync(MinerSign minerSign);
        Task DeleteAsync(MinerData data);
        Task DeleteAsync(MinerData[] datas);
    }
}
