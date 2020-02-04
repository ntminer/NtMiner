using LiteDB;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Data.Impl {
    public class MineWorkSet : IMineWorkSet {
        private readonly Dictionary<Guid, MineWorkData> _dicById = new Dictionary<Guid, MineWorkData>();

        private readonly IHostRoot _root;
        public MineWorkSet(IHostRoot root) {
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
                        var col = db.GetCollection<MineWorkData>();
                        foreach (var item in col.FindAll()) {
                            _dicById.Add(item.Id, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(Guid workId) {
            InitOnece();
            return _dicById.ContainsKey(workId);
        }

        public MineWorkData GetMineWork(Guid workId) {
            InitOnece();
            if (_dicById.ContainsKey(workId)) {
                return _dicById[workId];
            }
            return null;
        }

        public List<MineWorkData> GetAll() {
            InitOnece();
            return _dicById.Values.ToList();
        }

        public void AddOrUpdate(MineWorkData data) {
            InitOnece();
            lock (_locker) {
                using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                    var col = db.GetCollection<MineWorkData>();
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

        public void Remove(Guid id) {
            InitOnece();
            lock (_locker) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                        var col = db.GetCollection<MineWorkData>();
                        col.Delete(id);
                    }
                    SpecialPath.DeleteMineWorkFiles(id);
                }
            }
        }
    }
}
