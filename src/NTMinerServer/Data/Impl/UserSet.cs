using LiteDB;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;

namespace NTMiner.Data.Impl {
    public class UserSet : IUserSet {
        private Dictionary<string, IUser> _pubKeys = new Dictionary<string, IUser>();

        private IHostRoot _root;
        public UserSet(IHostRoot root) {
            _root = root;
        }

        private object _locker = new object();
        private DateTime _lastInitOn = DateTime.MinValue;

        private void InitOnece() {
            DateTime now = DateTime.Now;
            if (_lastInitOn.AddMinutes(10) < now) {
                lock (_locker) {
                    if (_lastInitOn.AddMinutes(10) < now) {
                        _pubKeys.Clear();
                        using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                            var col = db.GetCollection<UserData>();
                            foreach (var item in col.FindAll()) {
                                _pubKeys.Add(item.LoginName, item);
                            }
                        }
                        _lastInitOn = DateTime.Now;
                    }
                }
            }
        }

        public bool Contains(string pubKey) {
            InitOnece();
            return _pubKeys.ContainsKey(pubKey);
        }

        public bool TryGetKey(string pubKey, out IUser key) {
            InitOnece();
            return _pubKeys.TryGetValue(pubKey, out key);
        }
    }
}
