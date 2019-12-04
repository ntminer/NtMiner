using LiteDB;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Data.Impl {
    public class NTMinerWalletSet : INTMinerWalletSet {
        private readonly Dictionary<Guid, NTMinerWalletData> _dicById = new Dictionary<Guid, NTMinerWalletData>();

        private readonly IHostRoot _root;
        public NTMinerWalletSet(IHostRoot root) {
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
                        var col = db.GetCollection<NTMinerWalletData>();
                        foreach (var item in col.FindAll()) {
                            _dicById.Add(item.Id, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(Guid id) {
            InitOnece();
            return _dicById.ContainsKey(id);
        }

        public NTMinerWalletData GetNTMinerWallet(Guid id) {
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
            lock (_locker) {
                using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                    var col = db.GetCollection<NTMinerWalletData>();
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

        public void Remove(Guid id) {
            InitOnece();
            lock (_locker) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                        var col = db.GetCollection<NTMinerWalletData>();
                        col.Delete(id);
                    }
                }
            }
        }
    }
}
