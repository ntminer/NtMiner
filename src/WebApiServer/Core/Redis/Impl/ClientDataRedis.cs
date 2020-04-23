using NTMiner.Core.MinerServer;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class ClientDataRedis : IClientDataRedis {
        protected const string _redisKeyClientDataById = "clientDatas.ClientDataById";// 根据Id索引ClientData对象的json

        protected readonly ConnectionMultiplexer _connection;
        public ClientDataRedis(ConnectionMultiplexer connection) {
            _connection = connection;
        }

        public Task<List<ClientData>> GetAllAsync() {
            var db = _connection.GetDatabase();
            return db.HashGetAllAsync(_redisKeyClientDataById).ContinueWith(t => {
                List<ClientData> list = new List<ClientData>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        ClientData data = VirtualRoot.JsonSerializer.Deserialize<ClientData>(item.Value);
                        if (data != null) {
                            list.Add(data);
                        }
                    }
                }
                return list;
            });
        }

        public Task<ClientData> GetByIdAsync(string minerId) {
            if (string.IsNullOrEmpty(minerId)) {
                return Task.FromResult<ClientData>(null);
            }
            var db = _connection.GetDatabase();
            return db.HashGetAsync(_redisKeyClientDataById, minerId).ContinueWith(t => {
                if (t.Result.HasValue) {
                    return VirtualRoot.JsonSerializer.Deserialize<ClientData>(t.Result);
                }
                else {
                    return null;
                }
            });
        }

        public Task DeleteAsync(ClientData data) {
            if (data == null || string.IsNullOrEmpty(data.Id)) {
                return TaskEx.CompletedTask;
            }
            var db = _connection.GetDatabase();
            return db.HashSetAsync(_redisKeyClientDataById, data.Id, VirtualRoot.JsonSerializer.Serialize(data));
        }

        public Task SetAsync(ClientData data) {
            if (data == null || string.IsNullOrEmpty(data.Id)) {
                return TaskEx.CompletedTask;
            }
            var db = _connection.GetDatabase();
            return db.HashDeleteAsync(_redisKeyClientDataById, data.Id);
        }
    }
}
