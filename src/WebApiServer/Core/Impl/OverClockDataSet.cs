using LiteDB;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class OverClockDataSet : SetBase, IOverClockDataSet {
        private readonly Dictionary<Guid, OverClockData> _dicById = new Dictionary<Guid, OverClockData>();

        public OverClockDataSet() {
        }

        protected override void Init() {
            using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                var col = db.GetCollection<OverClockData>();
                foreach (var item in col.FindAll()) {
                    _dicById.Add(item.Id, item);
                }
            }
        }

        public void AddOrUpdate(OverClockData data) {
            InitOnece();
            lock (_dicById) {
                using (LiteDatabase db = AppRoot.CreateLocalDb()) {
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
            lock (_dicById) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                        var col = db.GetCollection<OverClockData>();
                        col.Delete(id);
                    }
                }
            }
        }
    }
}
