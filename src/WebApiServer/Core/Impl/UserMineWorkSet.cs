using LiteDB;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class UserMineWorkSet : SetBase, IUserMineWorkSet {
        private readonly Dictionary<Guid, UserMineWorkData> _dicById = new Dictionary<Guid, UserMineWorkData>();

        public UserMineWorkSet() {
        }

        protected override void Init() {
            using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                var col = db.GetCollection<UserMineWorkData>();
                foreach (var item in col.FindAll()) {
                    _dicById.Add(item.Id, item);
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
            lock (_dicById) {
                using (LiteDatabase db = AppRoot.CreateLocalDb()) {
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
            lock (_dicById) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                        var col = db.GetCollection<UserMineWorkData>();
                        col.Delete(id);
                    }
                    SpecialPath.DeleteMineWorkFiles(id);
                }
            }
        }
    }
}
