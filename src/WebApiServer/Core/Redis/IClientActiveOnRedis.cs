using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IClientActiveOnRedis {
        Task<Dictionary<string, DateTime>> GetAllAsync();
        Task SetAsync(string id, DateTime activeOn);
        Task DeleteAsync(string id);
    }
}
