using LiteDB;
using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Data.Impl {
    public class PoolSet : IPoolSet {
        private readonly Dictionary<Guid, PoolData> _dicById = new Dictionary<Guid, PoolData>();

        private readonly IHostRoot _root;

        public PoolSet(IHostRoot root) {
            _root = root;
        }

        private bool _isInited = false;
        private object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                        var col = db.GetCollection<PoolData>();
                        foreach (var item in col.FindAll()) {
                            _dicById.Add(item.Id, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public void AddOrUpdate(PoolData data) {
            InitOnece();
            lock (_locker) {
                using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                    var col = db.GetCollection<PoolData>();
                    if (_dicById.ContainsKey(data.Id)) {
                        _dicById[data.Id].Update(data);
                        col.Update(_dicById[data.Id]);
                    }
                    else {
                        _dicById.Add(data.Id, data);
                        col.Insert(data);
                    }
                }
            }
        }

        public List<PoolData> GetAll() {
            InitOnece();
            return _dicById.Values.ToList();
        }

        public void Remove(Guid id) {
            InitOnece();
            lock (_locker) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                        var col = db.GetCollection<PoolData>();
                        col.Delete(id);
                    }
                }
            }
        }
    }
}
