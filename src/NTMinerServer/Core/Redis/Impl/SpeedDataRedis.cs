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

        public Task<List<SpeedDto>> GetAllAsync() {
            var db = _connection.GetDatabase();
            return db.HashGetAllAsync(_redisKeySpeedDataByClientId).ContinueWith(t => {
                List<SpeedDto> list = new List<SpeedDto>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        SpeedDto data = VirtualRoot.JsonSerializer.Deserialize<SpeedDto>(item.Value);
                        if (data != null) {
                            list.Add(data);
                        }
                    }
                }
                return list;
            });
        }

        public Task<SpeedDto> GetByClientIdAsync(Guid clientId) {
            if (clientId == Guid.Empty) {
                return Task.FromResult<SpeedDto>(null);
            }
            var db = _connection.GetDatabase();
            return db.HashGetAsync(_redisKeySpeedDataByClientId, clientId.ToString()).ContinueWith(t => {
                if (t.Result.HasValue) {
                    return VirtualRoot.JsonSerializer.Deserialize<SpeedDto>(t.Result);
                }
                else {
                    return null;
                }
            });
        }

        public Task SetAsync(SpeedDto speedDto) {
            if (speedDto == null || speedDto.ClientId == Guid.Empty) {
                return TaskEx.CompletedTask;
            }
            var db = _connection.GetDatabase();
            return db.HashSetAsync(_redisKeySpeedDataByClientId, speedDto.ClientId.ToString(), VirtualRoot.JsonSerializer.Serialize(speedDto));
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
