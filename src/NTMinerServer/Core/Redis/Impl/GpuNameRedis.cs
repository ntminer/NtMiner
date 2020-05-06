using NTMiner.Core.Gpus;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class GpuNameRedis : IGpuNameRedis {
        protected const string _redisKeyGpuNameByClientId = "speedDatas.GpuNameByClientId";// 根据ClientId索引GpuName对象的json

        protected readonly ConnectionMultiplexer _connection;
        public GpuNameRedis(ConnectionMultiplexer connection) {
            _connection = connection;
        }

        public Task<List<GpuName>> GetAllAsync() {
            var db = _connection.GetDatabase();
            return db.HashGetAllAsync(_redisKeyGpuNameByClientId).ContinueWith(t => {
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
            // 忽略显存小于2G的卡
            if (gpuName == null || string.IsNullOrEmpty(gpuName.Name) || gpuName.TotalMemory < 2 * NTKeyword.ULongG) {
                return TaskEx.CompletedTask;
            }
            var db = _connection.GetDatabase();
            return db.HashSetAsync(_redisKeyGpuNameByClientId, gpuName.ToString(), VirtualRoot.JsonSerializer.Serialize(gpuName));
        }
    }
}
