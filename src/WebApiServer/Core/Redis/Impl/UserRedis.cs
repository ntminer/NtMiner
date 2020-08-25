using NTMiner.User;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class UserRedis : ReadOnlyUserRedis, IUserRedis {
        public UserRedis(IServerConnection serverConfig) : base(serverConfig) {
        }

        public Task SetAsync(UserData data) {
            if (data == null || string.IsNullOrEmpty(data.LoginName)) {
                return TaskEx.CompletedTask;
            }
            var db = _serverConnection.RedisConn.GetDatabase();
            return db.HashSetAsync(_redisKeyUserByLoginName, data.LoginName, VirtualRoot.JsonSerializer.Serialize(data));
        }

        public Task DeleteAsync(UserData data) {
            if (data == null) {
                return TaskEx.CompletedTask;
            }
            var db = _serverConnection.RedisConn.GetDatabase();
            return db.HashDeleteAsync(_redisKeyUserByLoginName, data.LoginName);
        }
    }
}
