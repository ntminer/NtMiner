using NTMiner.User;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class UserRedis : ReadOnlyUserRedis, IUserRedis {
        public UserRedis(ConnectionMultiplexer connection) : base(connection) {
        }

        public Task SetAsync(UserData data) {
            if (data == null || string.IsNullOrEmpty(data.LoginName)) {
                return TaskEx.CompletedTask;
            }
            var db = _connection.GetDatabase();
            return db.HashSetAsync(_redisKeyUserByLoginName, data.LoginName, VirtualRoot.JsonSerializer.Serialize(data));
        }

        public Task DeleteAsync(UserData data) {
            if (data == null) {
                return TaskEx.CompletedTask;
            }
            var db = _connection.GetDatabase();
            return db.HashDeleteAsync(_redisKeyUserByLoginName, data.LoginName);
        }
    }
}
