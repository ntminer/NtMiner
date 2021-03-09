using NTMiner.Core.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class CaptchaSet : ICaptchaSet {
        private readonly Dictionary<Guid, CaptchaData> _dicById = new Dictionary<Guid, CaptchaData>();
        private readonly object _locker = new object();

        public bool IsReadied {
            get; private set;
        }

        public int Count {
            get {
                return _dicById.Count;
            }
        }

        private readonly ICaptchaDataRedis _redis;
        public CaptchaSet(ICaptchaDataRedis redis) {
            _redis = redis;
            redis.GetAllAsync().ContinueWith(t => {
                if (t.Result != null && t.Result.Count != 0) {
                    foreach (var item in t.Result) {
                        _dicById.Add(item.Id, item);
                    }
                }
                IsReadied = true;
            });
            VirtualRoot.BuildEventPath<Per1MinuteEvent>("清理过期的验证码", LogEnum.DevConsole, path: message => {
                // 验证码在内存中留存10分钟
                DateTime time = message.BornOn.AddMinutes(-10);
                CaptchaData[] toRemoves;
                lock (_locker) {
                    toRemoves = _dicById.Values.Where(a => a.CreatedOn <= time).ToArray();
                }
                foreach (var item in toRemoves) {
                    _dicById.Remove(item.Id);
                    _redis.DeleteAsync(item);
                }
            }, this.GetType());
        }

        public bool SetCaptcha(CaptchaData data) {
            if (!IsReadied) {
                return false;
            }
            if (data == null || data.Id == Guid.Empty) {
                return false;
            }
            lock (_locker) {
                if (_dicById.ContainsKey(data.Id)) {
                    return false;
                }
                _dicById[data.Id] = data;
                _redis.SetAsync(data);
                return true;
            }
        }

        public bool IsValid(Guid id, string ip, string captcha) {
            if (!IsReadied) {
                return false;
            }
            if (id == Guid.Empty) {
                return false;
            }
            if (string.IsNullOrEmpty(captcha)) {
                return false;
            }
            if (!_dicById.TryGetValue(id, out CaptchaData item)) {
                return false;
            }
            if (!captcha.Equals(item.Code, StringComparison.OrdinalIgnoreCase)) {
                return false;
            }
            if (item.Ip != ip) {
                return false;
            }
            return true;
        }

        public int CountByIp(string ip) {
            if (string.IsNullOrEmpty(ip)) {
                throw new InvalidProgramException();
            }
            return _dicById.Values.Count(a => a.Ip == ip);
        }
    }
}
