using NTMiner.Core.MinerServer;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IMinerRedis : IReadOnlyMinerRedis {
        Task SetAsync(MinerData data);
        Task DeleteAsync(MinerData data);
    }
}
