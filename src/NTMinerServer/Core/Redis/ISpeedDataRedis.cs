using NTMiner.Report;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface ISpeedDataRedis {
        Task<Dictionary<Guid, SpeedData>> GetAllAsync();
        Task<SpeedData> GetByClientIdAsync(Guid clientId);
        Task SetAsync(SpeedData speedData);
        Task DeleteByClientIdAsync(Guid clientId);
    }
}
