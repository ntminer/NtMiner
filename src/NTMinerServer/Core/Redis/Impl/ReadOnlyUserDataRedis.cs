using NTMiner.User;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class ReadOnlyUserDataRedis : IReadOnlyUserDataRedis {
        protected const string _redisKeyUserByLoginName = RedisKeyword.UsersUserByLoginName;

        protected readonly IRedis _redis;
        public ReadOnlyUserDataRedis(IRedis redis) {
            _redis = redis;
        }

        public Task<List<UserData>> GetAllAsync() {
            var db = _redis.GetDatabase();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            return db.HashGetAllAsync(_redisKeyUserByLoginName).ContinueWith(t => {
                stopwatch.Stop();
                string text = $"{nameof(ReadOnlyUserDataRedis)}的redis方法HashGetAllAsync耗时 {stopwatch.GetElapsedSeconds().ToString("f2")} 秒";
                NTMinerConsole.UserInfo(text);
                stopwatch.Restart();
                List<UserData> list = new List<UserData>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        UserData data = VirtualRoot.JsonSerializer.Deserialize<UserData>(item.Value);
                        if (data != null) {
                            list.Add(data);
                        }
                    }
                }
                stopwatch.Stop();
                NTMinerConsole.UserInfo($"反序列化和装配UserData列表耗时 {stopwatch.GetElapsedSeconds().ToString("f2")} 秒");
                return list;
            });
        }

        public Task<UserData> GetByLoginNameAsync(string loginName) {
            if (string.IsNullOrEmpty(loginName)) {
                return Task.FromResult<UserData>(null);
            }
            var db = _redis.GetDatabase();
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
