using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IReadOnlyWsServerNodeRedis {
        Task<Dictionary<string, DateTime>> GetAllAddress();
    }
}
