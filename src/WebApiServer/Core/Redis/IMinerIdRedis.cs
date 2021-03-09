using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IMinerIdRedis {
        Task<List<KeyValuePair<Guid, string>>> GetAllAsync();
        Task<string> GetMinerIdByClientIdAsync(Guid clientId);
        Task SetAsync(Guid clientId, string minerId);
        Task DeleteByClientIdAsync(Guid clientId);
    }
}
