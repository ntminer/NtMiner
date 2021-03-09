using NTMiner.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IReadOnlyUserDataRedis {
        Task<List<UserData>> GetAllAsync();
        Task<UserData> GetByLoginNameAsync(string loginName);
    }
}
