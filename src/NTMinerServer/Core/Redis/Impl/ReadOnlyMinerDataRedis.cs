using NTMiner.Core.MinerServer;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class ReadOnlyMinerDataRedis : IReadOnlyMinerDataRedis {
        protected const string _redisKeyMinerById = RedisKeyword.MinersMinerById;

        protected readonly IRedis _redis;
        public ReadOnlyMinerDataRedis(IRedis redis) {
            _redis = redis;
        }

        public Task<List<MinerData>> GetAllAsync() {
            var db = _redis.GetDatabase();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            return db.HashGetAllAsync(_redisKeyMinerById).ContinueWith(t => {
                stopwatch.Stop();
                string text = $"{nameof(ReadOnlyMinerDataRedis)}的redis方法HashGetAllAsync耗时 {stopwatch.GetElapsedSeconds().ToString("f2")} 秒";
                NTMinerConsole.UserInfo(text);
                stopwatch.Restart();
                List<MinerData> list = new List<MinerData>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        MinerData data = VirtualRoot.JsonSerializer.Deserialize<MinerData>(item.Value);
                        if (data != null) {
                            list.Add(data);
                        }
                    }
                }
                stopwatch.Stop();
                NTMinerConsole.UserInfo($"反序列化和装配MinerData列表耗时 {stopwatch.GetElapsedSeconds().ToString("f2")} 秒");
                return list;
            });
        }

        public Task<MinerData> GetByIdAsync(string minerId) {
            if (string.IsNullOrEmpty(minerId)) {
                return Task.FromResult<MinerData>(null);
            }
            var db = _redis.GetDatabase();
            return db.HashGetAsync(_redisKeyMinerById, minerId).ContinueWith(t => {
                if (t.Result.HasValue) {
                    return VirtualRoot.JsonSerializer.Deserialize<MinerData>(t.Result);
                }
                else {
                    return null;
                }
            });
        }
    }
}
