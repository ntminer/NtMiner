using LiteDB;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class OverClockDataSet : IOverClockDataSet {
        private readonly Dictionary<Guid, OverClockData> _dicById = new Dictionary<Guid, OverClockData>();

        public OverClockDataSet() {
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    using (LiteDatabase db = WebApiRoot.CreateLocalDb()) {
                        var col = db.GetCollection<OverClockData>();
                        foreach (var item in col.FindAll()) {
                            _dicById.Add(item.Id, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public void AddOrUpdate(OverClockData data) {
            InitOnece();
            lock (_locker) {
                using (LiteDatabase db = WebApiRoot.CreateLocalDb()) {
                    var col = db.GetCollection<OverClockData>();
                    if (_dicById.TryGetValue(data.Id, out OverClockData entity)) {
                        entity.Update(data);
                        col.Update(entity);
                    }
                    else {
                        _dicById.Add(data.Id, data);
                        col.Insert(data);
                    }
                }
            }
        }

        public List<OverClockData> GetAll() {
            InitOnece();
            return _dicById.Values.ToList();
        }

        public void RemoveById(Guid id) {
            InitOnece();
            lock (_locker) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = WebApiRoot.CreateLocalDb()) {
                        var col = db.GetCollection<OverClockData>();
                        col.Delete(id);
                    }
                }
            }
        }
    }
}
