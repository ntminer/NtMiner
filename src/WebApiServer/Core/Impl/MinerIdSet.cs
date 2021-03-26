using NTMiner.Core.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NTMiner.Core.Impl {
    public class MinerIdSet : IMinerIdSet {
        private readonly Dictionary<Guid, string> _dicByClientId = new Dictionary<Guid, string>();

        public bool IsReadied {
            get; private set;
        }

        private readonly IMinerIdRedis _redis;
        public MinerIdSet(IMinerIdRedis redis) {
            _redis = redis;
            redis.GetAllAsync().ContinueWith(t => {
                if (t.Result != null && t.Result.Count != 0) {
                    foreach (var item in t.Result) {
                        _dicByClientId.Add(item.Key, item.Value);
                    }
                }
                IsReadied = true;
            });
            VirtualRoot.BuildEventPath<MinerDataAddedMqEvent>("将新增的矿机的MinerId载入内存", LogEnum.DevConsole, message => {
                _dicByClientId[message.ClientId] = message.MinerId;
            }, this.GetType());
            VirtualRoot.BuildEventPath<MinerDataRemovedMqEvent>("将删除的矿机的MinerId从内存移除", LogEnum.DevConsole, message => {
                _dicByClientId.Remove(message.ClientId);
            }, this.GetType());
        }

        public bool TryGetMinerId(Guid clientId, out string minerId) {
            if (!IsReadied) {
                minerId = string.Empty;
                return false;
            }
            return _dicByClientId.TryGetValue(clientId, out minerId);
        }

        public void Set(Guid clientId, string minerId) {
            if (!IsReadied) {
                return;
            }
            if (clientId == Guid.Empty || string.IsNullOrEmpty(minerId)) {
                return;
            }
            _dicByClientId[clientId] = minerId;
            _redis.SetAsync(clientId, minerId);
        }

        public void Remove(Guid clientId) {
            if (!IsReadied) {
                return;
            }
            _dicByClientId.Remove(clientId);
            _redis.DeleteByClientIdAsync(clientId);
        }

        public Task WaitReadiedAsync() {
            if (IsReadied) {
                return TaskEx.CompletedTask;
            }
            return Task.Factory.StartNew(() => {
                while (!IsReadied) {
                    Thread.Sleep(50);
                }
                return;
            });
        }
    }
}
