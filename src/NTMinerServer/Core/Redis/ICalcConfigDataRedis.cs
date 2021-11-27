using NTMiner.Core.MinerServer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface ICalcConfigDataRedis {
        Task<CalcConfigData[]> GetAllAsync();
        Task SetAsync(List<CalcConfigData> data);
    }
}
