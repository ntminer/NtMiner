using NTMiner.User;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface IUserDataRedis : IReadOnlyUserDataRedis {
        Task SetAsync(UserData data);
        Task DeleteAsync(UserData data);
    }
}
