using LiteDB;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class UserMineWorkSet : IUserMineWorkSet {
        private readonly Dictionary<Guid, UserMineWorkData> _dicById = new Dictionary<Guid, UserMineWorkData>();

        public UserMineWorkSet() {
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
                        var col = db.GetCollection<UserMineWorkData>();
                        foreach (var item in col.FindAll()) {
                            _dicById.Add(item.Id, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public UserMineWorkData GetById(Guid workId) {
            InitOnece();
            if (_dicById.ContainsKey(workId)) {
                return _dicById[workId];
            }
            return null;
        }

        public List<UserMineWorkData> GetsByLoginName(string loginName) {
            InitOnece();
            return _dicById.Values.Where(a => a.LoginName == loginName).ToList();
        }

        public void AddOrUpdate(UserMineWorkData data) {
            InitOnece();
            lock (_locker) {
                using (LiteDatabase db = WebApiRoot.CreateLocalDb()) {
                    var col = db.GetCollection<UserMineWorkData>();
                    if (_dicById.TryGetValue(data.Id, out UserMineWorkData entity)) {
                        data.ModifiedOn = DateTime.Now;
                        entity.Update(data);
                        col.Update(entity);
                    }
                    else {
                        data.CreatedOn = DateTime.Now;
                        _dicById.Add(data.Id, data);
                        col.Insert(data);
                    }
                }
            }
        }

        public void RemoveById(Guid id) {
            InitOnece();
            lock (_locker) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = WebApiRoot.CreateLocalDb()) {
                        var col = db.GetCollection<UserMineWorkData>();
                        col.Delete(id);
                    }
                    SpecialPath.DeleteMineWorkFiles(id);
                }
            }
        }
    }
}
