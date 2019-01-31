using LiteDB;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;

namespace NTMiner.User.Impl {
    public class UserSet : IUserSet {
        private Dictionary<string, IUser> _dicByLoginName = new Dictionary<string, IUser>();
        private readonly string _dbFileFullName;
        public UserSet(string dbFileFullName) {
            _dbFileFullName = dbFileFullName;
        }

        private object _locker = new object();
        private DateTime _lastInitOn = DateTime.MinValue;

        private void InitOnece() {
            DateTime now = DateTime.Now;
            if (_lastInitOn.AddMinutes(10) < now) {
                lock (_locker) {
                    if (_lastInitOn.AddMinutes(10) < now) {
                        _dicByLoginName.Clear();
                        using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                            var col = db.GetCollection<UserData>();
                            foreach (var item in col.FindAll()) {
                                _dicByLoginName.Add(item.LoginName, item);
                            }
                        }
                        _lastInitOn = DateTime.Now;
                    }
                }
            }
        }

        public bool Contains(string pubKey) {
            InitOnece();
            return _dicByLoginName.ContainsKey(pubKey);
        }

        public bool TryGetKey(string pubKey, out IUser key) {
            InitOnece();
            return _dicByLoginName.TryGetValue(pubKey, out key);
        }
    }
}
