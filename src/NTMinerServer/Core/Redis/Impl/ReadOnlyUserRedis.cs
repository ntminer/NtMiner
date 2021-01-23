using NTMiner.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class ReadOnlyUserRedis : IReadOnlyUserRedis {
        protected const string _redisKeyUserByLoginName = "users.UserByLoginName";// 根据LoginName索引User对象的json

        protected readonly IMqRedis _redis;
        public ReadOnlyUserRedis(IMqRedis redis) {
            _redis = redis;
        }

        public Task<List<UserData>> GetAllAsync() {
            var db = _redis.RedisConn.GetDatabase();
            return db.HashGetAllAsync(_redisKeyUserByLoginName).ContinueWith(t => {
                List<UserData> list = new List<UserData>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        UserData data = VirtualRoot.JsonSerializer.Deserialize<UserData>(item.Value);
                        if (data != null) {
                            list.Add(data);
                        }
                    }
                }
                return list;
            });
        }

        public Task<UserData> GetByLoginNameAsync(string loginName) {
            if (string.IsNullOrEmpty(loginName)) {
                return Task.FromResult<UserData>(null);
            }
            var db = _redis.RedisConn.GetDatabase();
            return db.HashGetAsync(_redisKeyUserByLoginName, loginName).ContinueWith(t => {
                if (t.Result.HasValue) {
                    return VirtualRoot.JsonSerializer.Deserialize<UserData>(t.Result);
                }
                else {
                    return null;
                }
            });
        }
    }
}
