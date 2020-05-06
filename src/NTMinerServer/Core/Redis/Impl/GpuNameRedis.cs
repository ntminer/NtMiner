using NTMiner.Core.Gpus;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class GpuNameRedis : IGpuNameRedis {
        private const string _redisKeyGpuNamesRaw = "gpuNames.Raw";

        protected readonly ConnectionMultiplexer _connection;
        public GpuNameRedis(ConnectionMultiplexer connection) {
            _connection = connection;
        }

        public Task<List<GpuName>> GetAllRawAsync() {
            var db = _connection.GetDatabase();
            return db.HashGetAllAsync(_redisKeyGpuNamesRaw).ContinueWith(t => {
                List<GpuName> list = new List<GpuName>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        GpuName data = VirtualRoot.JsonSerializer.Deserialize<GpuName>(item.Value);
                        if (data != null) {
                            list.Add(data);
                        }
                    }
                }
                return list;
            });
        }

        public Task SetRawAsync(GpuName gpuName) {
            // 忽略显存小于2G的卡
            if (gpuName == null || !gpuName.IsValid()) {
                return TaskEx.CompletedTask;
            }
            var db = _connection.GetDatabase();
            return db.HashSetAsync(_redisKeyGpuNamesRaw, gpuName.ToString(), VirtualRoot.JsonSerializer.Serialize(gpuName));
        }

        public Task SetRawAsync(List<GpuName> gpuNames) {
            if (gpuNames == null || gpuNames.Count == 0) {
                return TaskEx.CompletedTask;
            }
            gpuNames = gpuNames.Where(a => a.IsValid()).ToList();
            var db = _connection.GetDatabase();
            return db.HashSetAsync(_redisKeyGpuNamesRaw, gpuNames.Select(a => new HashEntry(a.ToString(), VirtualRoot.JsonSerializer.Serialize(a))).ToArray());
        }
    }
}
