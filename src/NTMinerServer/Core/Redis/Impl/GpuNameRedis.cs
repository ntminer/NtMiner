using NTMiner.Gpus;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class GpuNameRedis : IGpuNameRedis {
        protected const string _redisKeyGpuName = "gpuNames";
        protected readonly ConnectionMultiplexer _connection;
        public GpuNameRedis(ConnectionMultiplexer connection) {
            _connection = connection;
        }

        public Task<List<GpuName>> GetAllAsync() {
            var db = _connection.GetDatabase();
            return db.HashGetAllAsync(_redisKeyGpuName).ContinueWith(t => {
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

        public Task SetAsync(GpuName gpuName) {
            if (gpuName == null || !gpuName.IsValid()) {
                return TaskEx.CompletedTask;
            }
            var db = _connection.GetDatabase();
            return db.HashSetAsync(_redisKeyGpuName, gpuName.ToString(), VirtualRoot.JsonSerializer.Serialize(gpuName));
        }

        public Task DeleteAsync(GpuName gpuName) {
            if (gpuName == null || !gpuName.IsValid()) {
                return TaskEx.CompletedTask;
            }
            var db = _connection.GetDatabase();
            return db.HashDeleteAsync(_redisKeyGpuName, gpuName.ToString());
        }
    }
}
