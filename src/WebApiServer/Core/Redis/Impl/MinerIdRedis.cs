using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class MinerIdRedis : IMinerIdRedis {
        protected const string _redisKeyMinerIdByClientId = RedisKeyword.MinerIdByClientId;

        private readonly IRedis _redis;
        public MinerIdRedis(IRedis redis) {
            _redis = redis;
        }

        public Task<List<KeyValuePair<Guid, string>>> GetAllAsync() {
            var db = _redis.RedisConn.GetDatabase();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            return db.HashGetAllAsync(_redisKeyMinerIdByClientId).ContinueWith(t => {
                stopwatch.Stop();
                string text = $"{nameof(MinerIdRedis)}的redis方法HashGetAllAsync耗时 {stopwatch.GetElapsedSeconds().ToString("f2")} 秒";
                NTMinerConsole.UserInfo(text);
                stopwatch.Restart();
                List<KeyValuePair<Guid, string>> list = new List<KeyValuePair<Guid, string>>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue && Guid.TryParse(item.Name, out Guid clientId)) {
                        string minerId = item.Value;
                        if (!string.IsNullOrEmpty(minerId)) {
                            list.Add(new KeyValuePair<Guid, string>(clientId, minerId));
                        }
                    }
                }
                stopwatch.Stop();
                NTMinerConsole.UserInfo($"装配MinerId列表耗时 {stopwatch.GetElapsedSeconds().ToString("f2")} 秒");
                return list;
            });
        }

        public Task<string> GetMinerIdByClientIdAsync(Guid clientId) {
            if (clientId == Guid.Empty) {
                return TaskEx.FromResult(string.Empty);
            }
            var db = _redis.RedisConn.GetDatabase();
            return db.HashGetAsync(_redisKeyMinerIdByClientId, clientId.ToString()).ContinueWith(t => {
                if (t.Result.HasValue) {
                    string minerId = t.Result;
                    return minerId;
                }
                else {
                    return string.Empty;
                }
            });
        }

        public Task SetAsync(Guid clientId, string minerId) {
            if (clientId == Guid.Empty || string.IsNullOrEmpty(minerId)) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.RedisConn.GetDatabase();
            return db.HashSetAsync(_redisKeyMinerIdByClientId, clientId.ToString(), minerId);
        }

        public Task DeleteByClientIdAsync(Guid clientId) {
            if (clientId == Guid.Empty) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.RedisConn.GetDatabase();
            return db.HashDeleteAsync(_redisKeyMinerIdByClientId, clientId.ToString());
        }
    }
}
