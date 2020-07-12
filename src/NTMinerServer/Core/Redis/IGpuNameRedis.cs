using NTMiner.Gpus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IGpuNameRedis {
        Task<List<GpuName>> GetAllAsync();
        Task SetAsync(GpuName gpuName);
        Task DeleteAsync(GpuName gpuName);
    }
}
