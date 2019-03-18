using LiteDB;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Data.Impl {
    public class MinerGroupSet : IMinerGroupSet {
        private readonly Dictionary<Guid, MinerGroupData> _dicById = new Dictionary<Guid, MinerGroupData>();

        private readonly IHostRoot _root;
        public MinerGroupSet(IHostRoot root) {
            _root = root;
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
                    using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                        var col = db.GetCollection<MinerGroupData>();
                        foreach (var item in col.FindAll()) {
                            _dicById.Add(item.Id, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public void AddOrUpdate(MinerGroupData data) {
            InitOnece();
            lock (_locker) {
                using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                    var col = db.GetCollection<MinerGroupData>();
                    if (_dicById.ContainsKey(data.Id)) {
                        data.ModifiedOn = DateTime.Now;
                        _dicById[data.Id].Update(data);
                        col.Update(_dicById[data.Id]);
                    }
                    else {
                        data.CreatedOn = DateTime.Now;
                        _dicById.Add(data.Id, data);
                        col.Insert(data);
                    }
                }
            }
        }

        public MinerGroupData GetMinerGroup(Guid id) {
            InitOnece();
            MinerGroupData data;
            if (_dicById.TryGetValue(id, out data)) {
                return data;
            }
            return null;
        }

        public List<MinerGroupData> GetAll() {
            InitOnece();
            return _dicById.Values.ToList();
        }

        public void Remove(Guid id) {
            InitOnece();
            lock (_locker) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                        var col = db.GetCollection<MinerGroupData>();
                        col.Delete(id);
                    }
                }
            }
        }
    }
}
