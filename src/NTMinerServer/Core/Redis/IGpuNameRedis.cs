using NTMiner.Core.Gpus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IGpuNameRedis {
        Task<List<GpuName>> GetAllRawAsync();
        /// <summary>
        /// 系统自动维护GpuNameSet时走批量接口，可以认为这个接口是用于人工添加新上市的Gpu的。
        /// </summary>
        /// <param name="gpuName"></param>
        /// <returns></returns>
        Task SetRawAsync(GpuName gpuName);
        Task SetRawAsync(List<GpuName> gpuNames);
    }
}
