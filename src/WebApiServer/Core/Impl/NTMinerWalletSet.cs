using LiteDB;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class NTMinerWalletSet : SetBase, INTMinerWalletSet {
        private readonly Dictionary<Guid, NTMinerWalletData> _dicById = new Dictionary<Guid, NTMinerWalletData>();

        public NTMinerWalletSet() {
        }

        protected override void Init() {
            using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                var col = db.GetCollection<NTMinerWalletData>();
                foreach (var item in col.FindAll()) {
                    _dicById.Add(item.Id, item);
                }
            }
        }

        public bool Contains(Guid id) {
            InitOnece();
            return _dicById.ContainsKey(id);
        }

        public NTMinerWalletData GetById(Guid id) {
            InitOnece();
            if (_dicById.ContainsKey(id)) {
                return _dicById[id];
            }
            return null;
        }

        public List<NTMinerWalletData> GetAll() {
            InitOnece();
            return _dicById.Values.ToList();
        }

        public void AddOrUpdate(NTMinerWalletData data) {
            InitOnece();
            lock (_dicById) {
                using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                    var col = db.GetCollection<NTMinerWalletData>();
                    if (_dicById.TryGetValue(data.Id, out NTMinerWalletData entity)) {
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

        public void RemoveById(Guid id) {
            InitOnece();
            lock (_dicById) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                        var col = db.GetCollection<NTMinerWalletData>();
                        col.Delete(id);
                    }
                }
            }
        }
    }
}
