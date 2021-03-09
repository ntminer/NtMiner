using LiteDB;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class NTMinerFileSet : SetBase, INTMinerFileSet {
        private readonly Dictionary<Guid, NTMinerFileData> _dicById = new Dictionary<Guid, NTMinerFileData>();
        private DateTime _lastChangedOn = DateTime.MinValue;
        public NTMinerFileSet() {
        }

        protected override void Init() {
            using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                var col = db.GetCollection<NTMinerFileData>();
                foreach (var item in col.FindAll()) {
                    _dicById.Add(item.Id, item);
                }
            }
            CacheLatest();
        }

        public DateTime LastChangedOn {
            get {
                InitOnece();
                return _lastChangedOn;
            }
        }

        private NTMinerFileData _latestMinerClientFile;
        public NTMinerFileData LatestMinerClientFile {
            get {
                InitOnece();
                return _latestMinerClientFile;
            }
        }

        private NTMinerFileData _latestMinerStudioFile;
        public NTMinerFileData LatestMinerStudioFile {
            get {
                InitOnece();
                return _latestMinerStudioFile;
            }
        }

        public IEnumerable<NTMinerFileData> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }

        public void AddOrUpdate(NTMinerFileData data) {
            InitOnece();
            lock (_dicById) {
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
                    CacheLatest();
                }
            }
        }

        public void RemoveById(Guid id) {
            InitOnece();
            lock (_dicById) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                        var col = db.GetCollection<NTMinerFileData>();
                        col.Delete(id);
                    }
                    CacheLatest();
                }
            }
        }

        private void CacheLatest() {
            foreach (var item in _dicById.Values) {
                switch (item.AppType) {
                    case NTMinerAppType.MinerClient:
                        if (_latestMinerClientFile == null) {
                            _latestMinerClientFile = item;
                        }
                        else if (item.GetVersion() > _latestMinerClientFile.GetVersion()) {
                            _latestMinerClientFile = item;
                        }
                        break;
                    case NTMinerAppType.MinerStudio:
                        if (_latestMinerStudioFile == null) {
                            _latestMinerStudioFile = item;
                        }
                        else if (item.GetVersion() > _latestMinerStudioFile.GetVersion()) {
                            _latestMinerStudioFile = item;
                        }
                        break;
                    default:
                        break;
                }
            }
            _lastChangedOn = DateTime.Now;
        }
    }
}
