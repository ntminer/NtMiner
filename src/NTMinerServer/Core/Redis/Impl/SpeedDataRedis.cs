using NTMiner.Report;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class SpeedDataRedis : ISpeedDataRedis {
        protected const string _redisKeySpeedDataByClientId = "speedDatas.SpeedDataByClientId";// 根据ClientId索引SpeedData对象的json

        protected readonly ConnectionMultiplexer _connection;
        public SpeedDataRedis(ConnectionMultiplexer connection) {
            _connection = connection;
        }

        public Task<List<SpeedData>> GetAllAsync() {
            var db = _connection.GetDatabase();
            return db.HashGetAllAsync(_redisKeySpeedDataByClientId).ContinueWith(t => {
                List<SpeedData> list = new List<SpeedData>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        SpeedData data = VirtualRoot.JsonSerializer.Deserialize<SpeedData>(item.Value);
                        if (data != null) {
                            list.Add(data);
                        }
                    }
                }
                return list;
            });
        }

        public Task<SpeedData> GetByClientIdAsync(Guid clientId) {
            if (clientId == Guid.Empty) {
                return Task.FromResult<SpeedData>(null);
            }
            var db = _connection.GetDatabase();
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
            var db = _connection.GetDatabase();
            return db.HashSetAsync(_redisKeySpeedDataByClientId, speedData.ClientId.ToString(), VirtualRoot.JsonSerializer.Serialize(speedData));
        }

        public Task DeleteByClientIdAsync(Guid clientId) {
            if (clientId == Guid.Empty) {
                return TaskEx.CompletedTask;
            }
            var db = _connection.GetDatabase();
            return db.HashDeleteAsync(_redisKeySpeedDataByClientId, clientId.ToString());
        }
    }
}
