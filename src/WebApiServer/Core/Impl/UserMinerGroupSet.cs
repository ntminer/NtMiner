using LiteDB;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class UserMinerGroupSet : SetBase, IUserMinerGroupSet {
        private readonly Dictionary<Guid, UserMinerGroupData> _dicById = new Dictionary<Guid, UserMinerGroupData>();

        public UserMinerGroupSet() {
        }

        protected override void Init() {
            using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                var col = db.GetCollection<UserMinerGroupData>();
                foreach (var item in col.FindAll()) {
                    _dicById.Add(item.Id, item);
                }
            }
        }

        public void AddOrUpdate(UserMinerGroupData data) {
            InitOnece();
            lock (_dicById) {
                using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                    var col = db.GetCollection<UserMinerGroupData>();
                    if (_dicById.TryGetValue(data.Id, out UserMinerGroupData entity)) {
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

        public UserMinerGroupData GetById(Guid id) {
            InitOnece();
            if (_dicById.TryGetValue(id, out UserMinerGroupData data)) {
                return data;
            }
            return null;
        }

        public List<UserMinerGroupData> GetsByLoginName(string loginName) {
            InitOnece();
            return _dicById.Values.Where(a => a.LoginName == loginName).ToList();
        }

        public void RemoveById(Guid id) {
            InitOnece();
            lock (_dicById) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                        var col = db.GetCollection<UserMinerGroupData>();
                        col.Delete(id);
                    }
                }
            }
        }
    }
}
