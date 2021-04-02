using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class CaptchaDataRedis : ICaptchaDataRedis {
        protected const string _redisKeyCaptchaById = RedisKeyword.CaptchasCaptchaById;

        protected readonly IRedis _redis;
        public CaptchaDataRedis(IRedis redis) {
            _redis = redis;
        }

        public Task<List<CaptchaData>> GetAllAsync() {
            var db = _redis.GetDatabase();
            return db.HashGetAllAsync(_redisKeyCaptchaById).ContinueWith(t => {
                List<CaptchaData> list = new List<CaptchaData>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        CaptchaData data = VirtualRoot.JsonSerializer.Deserialize<CaptchaData>(item.Value);
                        if (data != null) {
                            list.Add(data);
                        }
                    }
                }
                return list;
            });
        }

        public Task SetAsync(CaptchaData data) {
            if (data == null || data.Id == Guid.Empty) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.GetDatabase();
            return db.HashSetAsync(_redisKeyCaptchaById, data.Id.ToString(), VirtualRoot.JsonSerializer.Serialize(data));
        }

        public Task DeleteAsync(CaptchaData data) {
            if (data == null || data.Id == Guid.Empty) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.GetDatabase();
            return db.HashDeleteAsync(_redisKeyCaptchaById, data.Id.ToString());
        }
    }
}
