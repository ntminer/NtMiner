using LiteDB;
using NTMiner.Core;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class NTMinerFileSet : INTMinerFileSet {
        private readonly Dictionary<Guid, NTMinerFileData> _dicById = new Dictionary<Guid, NTMinerFileData>();

        public NTMinerFileSet() {
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
                    using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                        var col = db.GetCollection<NTMinerFileData>();
                        foreach (var item in col.FindAll()) {
                            _dicById.Add(item.Id, item);
                        }
                    }
                    RefreshLatest();
                    _isInited = true;
                }
            }
        }

        private NTMinerFileData _latestMinerClientFile;
        public NTMinerFileData LatestMinerClientFile {
            get {
                InitOnece();
                return _latestMinerClientFile;
            }
        }

        private void RefreshLatest() {
            _latestMinerClientFile = _dicById.Values.Where(a => a.AppType == NTMinerAppType.MinerClient).OrderByDescending(a => a.GetVersion()).FirstOrDefault();
        }

        public void AddOrUpdate(NTMinerFileData data) {
            InitOnece();
            lock (_locker) {
                using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                    var col = db.GetCollection<NTMinerFileData>();
                    if (_dicById.TryGetValue(data.Id, out NTMinerFileData entity)) {
                        entity.Update(data);
                        col.Update(entity);
                    }
                    else {
                        data.CreatedOn = DateTime.Now;
                        _dicById.Add(data.Id, data);
                        col.Insert(data);
                    }
                    RefreshLatest();
                }
            }
        }

        public List<NTMinerFileData> GetAll() {
            InitOnece();
            return _dicById.Values.ToList();
        }

        public void RemoveById(Guid id) {
            InitOnece();
            lock (_locker) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                        var col = db.GetCollection<NTMinerFileData>();
                        col.Delete(id);
                    }
                    RefreshLatest();
                }
            }
        }
    }
}
