using NTMiner.Report;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class SpeedDataRedis : ISpeedDataRedis {
        protected const string _redisKeySpeedDataByClientId = "speedDatas.SpeedDataByClientId";// 根据ClientId索引SpeedData对象的json

        protected readonly IMqRedis _redis;
        public SpeedDataRedis(IMqRedis redis) {
            _redis = redis;
        }

        public Task<List<SpeedData>> GetAllAsync() {
            var db = _redis.RedisConn.GetDatabase();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            return db.HashGetAllAsync(_redisKeySpeedDataByClientId).ContinueWith(t => {
                stopwatch.Stop();
                long seconds = stopwatch.ElapsedMilliseconds / 1000;
                string text = $"{nameof(SpeedDataRedis)}的redis方法HashGetAllAsync耗时 {seconds.ToString()} 秒";
                // 从redis一次性加载几十兆数据没有什么问题，打印些统计信息出来以待将来有问题时容易发现
                if (seconds > 5) {
                    NTMinerConsole.UserWarn(text);
                }
                else {
                    NTMinerConsole.UserInfo(text);
                }
                stopwatch.Start();
                List<SpeedData> list = new List<SpeedData>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        SpeedData data = VirtualRoot.JsonSerializer.Deserialize<SpeedData>(item.Value);
                        if (data != null) {
                            list.Add(data);
                        }
                    }
                }
                stopwatch.Stop();
                seconds = stopwatch.ElapsedMilliseconds / 1000;
                NTMinerConsole.UserInfo($"反序列化和装配耗时 {seconds.ToString()} 秒");
                return list;
            });
        }

        public Task<SpeedData> GetByClientIdAsync(Guid clientId) {
            if (clientId == Guid.Empty) {
                return Task.FromResult<SpeedData>(null);
            }
            var db = _redis.RedisConn.GetDatabase();
            return db.HashGetAsync(_redisKeySpeedDataByClientId, clientId.ToString()).ContinueWith(t => {
                if (t.Result.HasValue) {
                    return VirtualRoot.JsonSerializer.Deserialize<SpeedData>(t.Result);
                }
                else {
                    return null;
                }
            });
        }

        public Task SetAsync(SpeedData speedData) {
            if (speedData == null || speedData.ClientId == Guid.Empty) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.RedisConn.GetDatabase();
            return db.HashSetAsync(_redisKeySpeedDataByClientId, speedData.ClientId.ToString(), VirtualRoot.JsonSerializer.Serialize(speedData));
        }

        public Task DeleteByClientIdsAsync(Guid[] clientIds) {
            if (clientIds == null || clientIds.Length == 0) {
                return TaskEx.CompletedTask;
            }
            var values = new StackExchange.Redis.RedisValue[clientIds.Length];
            for (int i = 0; i < clientIds.Length; i++) {
                values[i] = clientIds[i].ToString();
            }
            var db = _redis.RedisConn.GetDatabase();
            return db.HashDeleteAsync(_redisKeySpeedDataByClientId, values);
        }
    }
}
