using LiteDB;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.User.Impl {
    public class UserSet : IUserSet {
        private Dictionary<string, UserData> _dicByLoginName = new Dictionary<string, UserData>();

        private readonly string _dbFileFullName;
        public UserSet(string dbFileFullName) {
            _dbFileFullName = dbFileFullName;
        }

        private bool _isInited = false;
        private object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                        var col = db.GetCollection<UserData>();
                        _dicByLoginName = col.FindAll().ToDictionary(a => a.LoginName, a => a);
                    }
                    _isInited = true;
                }
            }
        }

        public void ReInit() {
            _isInited = false;
        }

        public bool TryGetUser(string loginName, out IUser user) {
            InitOnece();
            UserData userData;
            bool result = _dicByLoginName.TryGetValue(loginName, out userData);
            user = userData;
            return result;
        }

        public IEnumerator<IUser> GetEnumerator() {
            InitOnece();
            return _dicByLoginName.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicByLoginName.Values.GetEnumerator();
        }
    }
}
