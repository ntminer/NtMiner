using NTMiner.Report;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface ISpeedDataRedis {
        Task<List<SpeedDto>> GetAllAsync();
        Task<SpeedDto> GetByClientIdAsync(Guid clientId);
        Task SetAsync(SpeedDto speedDto);
        Task DeleteByClientIdAsync(Guid clientId);
    }
}
