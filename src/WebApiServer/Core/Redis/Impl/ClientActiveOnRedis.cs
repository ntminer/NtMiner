using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class ClientActiveOnRedis : IClientActiveOnRedis {
        protected const string _redisKeyClientActiveOnById = RedisKeyword.ClientActiveOnById;

        protected readonly IRedis _redis;
        public ClientActiveOnRedis(IRedis redis) {
            _redis = redis;
        }

        public Task<Dictionary<string, DateTime>> GetAllAsync() {
            var db = _redis.RedisConn.GetDatabase();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            return db.HashGetAllAsync(_redisKeyClientActiveOnById).ContinueWith(t => {
                stopwatch.Stop();
                string text = $"{nameof(ClientActiveOnRedis)}的redis方法HashGetAllAsync耗时 {stopwatch.GetElapsedSeconds().ToString("f2")} 秒";
                NTMinerConsole.UserInfo(text);
                stopwatch.Restart();
                Dictionary<string, DateTime> dic = new Dictionary<string, DateTime>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        // 为了在redis-cli的易读性而存字符串
                        if (!DateTime.TryParse(item.Value, out DateTime activeOn)) {
                            activeOn = DateTime.MinValue;
                        }
                        dic.Add(item.Name, activeOn);
                    }
                }
                stopwatch.Stop();
                NTMinerConsole.UserInfo($"装配ClientActiveOn字典耗时 {stopwatch.GetElapsedSeconds().ToString("f2")} 秒");
                return dic;
            });
        }

        public Task SetAsync(string id, DateTime activeOn) {
            if (string.IsNullOrEmpty(id)) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.RedisConn.GetDatabase();
            // 为了在redis-cli的易读性而存字符串
            return db.HashSetAsync(_redisKeyClientActiveOnById, id, activeOn.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public Task DeleteAsync(string id) {
            if (string.IsNullOrEmpty(id)) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.RedisConn.GetDatabase();
            return db.HashDeleteAsync(_redisKeyClientActiveOnById, id);
        }
    }
}
